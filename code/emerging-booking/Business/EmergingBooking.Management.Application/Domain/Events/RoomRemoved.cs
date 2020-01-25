﻿using EmergingBooking.Infrastructure.Cqrs.Domain;

namespace EmergingBooking.Reservation.Application.Domain.Events
{
    public sealed class RoomRemoved : IDomainEvent
    {
        public RoomRemoved()
        {
        }
    }
}