module Singer

open System
open RoomNumber
open BookingDateRange

// Note: syntax & intellisense drive me crazy
type SingerId = SingerId of int
type SingerName = SingerName of string
type SingerPrice = SingerPrice of int

type SingerParticipation = {
    Name: SingerName
    Price: SingerPrice
    Date: DateOnly
}

type SingerAvailability =
    | SingerParticipating of SingerParticipation
    | SingerHasNoTime
type FindAvailableSinger = RoomNumber -> BookingDateRange -> SingerAvailability
