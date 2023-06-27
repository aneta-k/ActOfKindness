﻿using Application.Dtos.Event;
using Application.Dtos.User;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<DetailsEventDto>>> GetEvents()
        {
            return await _eventService.GetEvents();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CreateEvent([FromBody]CreateEventDto newEvent)
        {
            await _eventService.CreateEvent(newEvent);

            return Ok();
        }

        [AllowAnonymous]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteEvent([FromRoute]Guid id)
        {
            await _eventService.DeleteEvent(id);

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<DetailsEventDto>> GetEventById([FromRoute]Guid id)
        {
            var eventDto = await _eventService.GetEventById(id);
            //var userDto = await _eventService.GetUserById(eventDto.UserId);
            return eventDto;
        }

        [AllowAnonymous]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateEvent([FromRoute]Guid id, [FromBody]EditEventDto eventDto)
        {
            await _eventService.UpdateEvent(id, eventDto);

            return Ok();
        }

    }
}
