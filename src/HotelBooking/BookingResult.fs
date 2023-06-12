module BookingResult

open BookingDateRange
open Singer

type DeclineReason =
    | RoomUnavailable
    | SingerUnavailable
    | PaymentDeclined

type CustomerNotification = {
    Message: string
    // Note: I'm too lazy to add other properties
}

type BookingEvent =
    | SingerInvited of SingerParticipation
    | CustomerNotified of CustomerNotification

type AcceptedBooking = {
    DateRange: BookingDateRange
    BookingEvents: BookingEvent list
}

type BookingResult =
    | BookingAccepted of AcceptedBooking
    | BookingDeclined of DeclineReason
