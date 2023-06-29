﻿using Application.Dtos.Event;
using Application.Dtos.User;
using Application.Interfaces;
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
        public async Task<ActionResult<List<DetailsEventDto>>> GetModeratedEvents()
        {
            return await _eventService.GetModeratedEvents();
        }

        [AllowAnonymous]
        [HttpGet("unmoderated")]
        public async Task<ActionResult<List<DetailsEventDto>>> GetUnmoderatedEvents()
        {
            return await _eventService.GetUnmoderatedEvents();
        }

        [AllowAnonymous]
        [HttpGet("/location={location:string}&type={type:string}&startingDate={startingDate:string}&endingDate={endingDate:string}")]
        public async Task<ActionResult<List<DetailsEventDto>>> GetFilteredEvents()
        {
            return await _eventService.GetFilteredEvents();
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
            return eventDto;
        }

        [AllowAnonymous]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateEvent([FromRoute]Guid id, [FromBody]EditEventDto eventDto)
        {
            await _eventService.UpdateEvent(id, eventDto);

            return Ok();
        }

        [AllowAnonymous]
        [HttpPatch("{id:guid}/moderate")]
        public async Task<ActionResult> ModerateEvent([FromRoute] Guid id)
        {
            await _eventService.ModerateEvent(id);

            return Ok();
        }
    }
}
