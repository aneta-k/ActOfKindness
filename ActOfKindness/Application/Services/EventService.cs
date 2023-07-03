﻿using Application.Dtos.Event;
using Application.Exceptions;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;
        private readonly IContextService _contextService;
        private readonly UserManager<AppUser> _userManager;

        public EventService(IEventRepository eventRepository, IMapper mapper, IContextService contextService, UserManager<AppUser> userManager)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
            _contextService = contextService;
            _userManager = userManager;
        }

        public async Task<List<DetailsEventDto>> GetModeratedEventsAsync()
        {
            var events = await _eventRepository.GetModeratedEventsAsync();

            var eventsDto = _mapper.Map<List<DetailsEventDto>>(events);
            Log.Information("Get all moderated events => {eventsDto}");
            return eventsDto;
        }

        public async Task<List<DetailsEventDto>> GetUnmoderatedEventsAsync()
        {
            var unmoderatedEvents = await _eventRepository.GetUnmoderatedEventsAsync();
            var unmoderatedEventsDto = _mapper.Map<List<DetailsEventDto>>(unmoderatedEvents);

            return unmoderatedEventsDto;
        }

        public async Task CreateEventAsync(CreateEventDto newEventDto)
        {
            if (await _eventRepository.GetEventByIdAsync(newEventDto.Id) is not null)
                throw new BadRequestException(
                    $"Event with this ID ({newEventDto.Id}) exist",
                    _contextService.Method,
                    _contextService.GetUserId,
                    _contextService.GetUserRole);

            var temporaryImagePlaceHolder =
                "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3c/Vue_de_nuit_de_la_Place_Stanislas_%C3%A0_Nancy.jpg/1920px-Vue_de_nuit_de_la_Place_Stanislas_%C3%A0_Nancy.jpg";

            newEventDto.Image ??= temporaryImagePlaceHolder;

            var newEvent = _mapper.Map<Event>(newEventDto);

            newEvent.UserId = _contextService.GetUserId;

            await _eventRepository.CreateEventAsync(newEvent);
            await _eventRepository.SaveAsync();
        }

        public async Task DeleteEventAsync(Guid id)
        {
            var eventToDelete = await _eventRepository.GetEventByIdAsync(id);

            var userId = _contextService.GetUserId;
            var role = _contextService.GetUserRole;

            if (eventToDelete is null) throw new NotFoundException(
                $"Event ({id}) not found",
                _contextService.Method,
                userId,
                role);

            if (eventToDelete.UserId != userId && role == "User") throw new ForbidException(
                $"You are not allowed to delete event ({id})",
                _contextService.Method,
                userId,
                role);

            await _eventRepository.DeleteEventAsync(id);
        }

        public async Task<DetailsEventDto> GetEventByIdAsync(Guid id)
        {
            var eventDetails = await _eventRepository.GetEventByIdAsync(id);

            if (eventDetails is null) throw new NotFoundException(
                $"Event ({id}) not found",
                _contextService.Method,
                _contextService.GetUserId,
                _contextService.GetUserRole);

            var eventDetailsDto = _mapper.Map<DetailsEventDto>(eventDetails);

            return eventDetailsDto;
        }

        public async Task UpdateEventAsync(Guid id, EditEventDto updatedEventDto)
        {
            var eventToUpdate = await _eventRepository.GetEventByIdAsync(id);
            
            var userId = _contextService.GetUserId;
            var role = _contextService.GetUserRole;

            if (eventToUpdate is null) throw new NotFoundException(
                $"Event ({id}) not found",
                _contextService.Method,
                userId,
                role);

            if (eventToUpdate.UserId != userId && role == "User") throw new ForbidException(
                $"You are not allowed to update event ({id})",
                _contextService.Method,
                userId,
                role);

            await _eventRepository.UpdateEventAsync(id, updatedEventDto);
        }

        public async Task ModerateEventAsync(Guid id)
        {
            var rowsChanged = await _eventRepository.ModerateEventAsync(id);

            if (rowsChanged == 0) throw new NotFoundException(
                $"Event ({id}) not found",
                _contextService.Method,
                _contextService.GetUserId,
                _contextService.GetUserRole);
        }

        public async Task<List<DetailsEventDto>> GetFilteredModeratedEventsAsync(EventFilter filter)
        {
            var events = await _eventRepository.GetFilteredModeratedEventsAsync(filter);

            var eventsDto = _mapper.Map<List<DetailsEventDto>>(events);

            return eventsDto;
        }

        public async Task<List<ParticipantDto>> ReturnParticipantsDto(Guid eventId)
        {
            var newEvent =  await _eventRepository.GetEventByIdAsync(eventId);
            var listOfParticipants = new List<ParticipantDto>();
            foreach (var newEventParticipant in newEvent.Participants)
            {
                var newParticipant = new ParticipantDto
                {
                    UserName = newEventParticipant.User.UserName,
                    Location = newEventParticipant.User.Location,
                    Avatar = "https://api.multiavatar.com/Binx Bond.png"
                };
                listOfParticipants.Add(newParticipant);
            }
            return listOfParticipants;
        }

        public async Task JoinEventAsync(Guid eventId)
        {
            var eventToJoin = await _eventRepository.GetEventByIdAsync(eventId);
            var userId =  _contextService.GetUserId;
            if (userId is not null)
            {
                var userWhoWantsToJoin = await _userManager.FindByIdAsync(userId);


                if (eventToJoin is not null && userWhoWantsToJoin is not null)
                {
                    var eventUser = new EventUser
                    {
                        Event = eventToJoin,
                        EventId = eventToJoin.Id,
                        User = userWhoWantsToJoin,
                        UserId = userWhoWantsToJoin.Id

                    };
                    eventToJoin.Participants.Add(eventUser);
                }
            }

            await _eventRepository.SaveAsync();
        }

  
    }
}
