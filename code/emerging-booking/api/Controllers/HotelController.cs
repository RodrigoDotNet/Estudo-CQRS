﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EmergingBooking.Infrastructure.Cqrs.Commands;
using EmergingBooking.Infrastructure.Cqrs.Queries;
using EmergingBooking.Management.Application.Commands;
using EmergingBooking.Queries.Application.Hotel.Query;
using EmergingBooking.Queries.Application.Hotel.ReadModel;
using EmergingBookingApi.InputModel.Management;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace EmergingBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandDispatcher _commandDispatcher;

        public HotelController(
            ICommandDispatcher commandDispatcher,
            IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
            _commandDispatcher = commandDispatcher;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<HotelListItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var result = await _queryProcessor.ExecuteQueryAsync<HotelQuery, IEnumerable<HotelListItem>>(new HotelQuery());
            return Ok(result);
        }

        [HttpGet("{hotelCode:guid}/rooms")]
        [ProducesResponseType(typeof(IEnumerable<RoomListItem>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid hotelCode)
        {
            var result = await _queryProcessor.ExecuteQueryAsync<RoomQuery, IEnumerable<RoomListItem>>(new RoomQuery(hotelCode));
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Post(Hotel hotel)
        {
            var result = await _commandDispatcher.ExecuteAsync(
                            new CreateHotel(hotel.Name,
                                            hotel.StarsOfCategory,
                                            hotel.Street,
                                            hotel.District,
                                            hotel.City,
                                            hotel.Country,
                                            hotel.Zipcode,
                                            hotel.Email,
                                            hotel.Phone,
                                            hotel.Mobile));

            if (result.Failure)
            {
                return UnprocessableEntity(result);
            }

            return Created("", result);
        }

        [HttpPatch("{hotelCode:guid}/address/update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PatchHotelAddress(Guid hotelCode, HotelAddress hotelAddress)
        {
            var result = await _commandDispatcher.ExecuteAsync(
                new UpdateHotelAddress(hotelCode,
                                       hotelAddress.Street,
                                       hotelAddress.District,
                                       hotelAddress.City,
                                       hotelAddress.Country,
                                       hotelAddress.Zipcode));

            if (result.Failure)
            {
                return UnprocessableEntity(result);
            }

            return NoContent();
        }

        [HttpPatch("{hotelCode:guid}/contacts/update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> PatchHotelContacts(Guid hotelCode, HotelContacts hotelContacts)
        {
            var result = await _commandDispatcher.ExecuteAsync(
                new UpdateHotelContacts(hotelCode,
                                        hotelContacts.Email,
                                        hotelContacts.Phone,
                                        hotelContacts.Mobile));

            if (result.Failure)
            {
                return UnprocessableEntity(result);
            }

            return NoContent();
        }

        [HttpPost("{hotelCode:guid}/room")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddHotelRoom(Guid hotelCode, [FromBody]HotelRoom hotelRoom)
        {
            var result = await _commandDispatcher.ExecuteAsync(
                new AddRoomToHotel(hotelCode,
                                   hotelRoom.Name,
                                   hotelRoom.Description,
                                   hotelRoom.Capacity,
                                   hotelRoom.AvailableQuantity,
                                   hotelRoom.PricePerNight,
                                   hotelRoom.Amenities));

            if (result.Failure)
            {
                return UnprocessableEntity(result);
            }

            return NoContent();
        }
    }
}