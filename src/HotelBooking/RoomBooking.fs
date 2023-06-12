module RoomBooking

open RoomNumber
open BookingDateRange
open Singer
open BookingRequest
open BookingResult

type CheckRoomAvailability = BookingRequest -> bool

type Booking = {
    RoomNumber: RoomNumber
    BookingDateRange: BookingDateRange
    Singer: SingerParticipation option
}
// Note: we could use here PaymentResult, ...Result, but for brevity I used bool
type Charge = Booking -> bool

// Note: cannot start with workflow before the types are complete
type BookRoomWorkflow =
    CheckRoomAvailability           // dependency
        -> FindAvailableSinger      // dependency
        -> Charge                   // dependency
        -> BookingRequest           // input
        -> BookingResult            // output
        
// Note: imagine pipeline
// 1. BookingRequest ->  CheckRoomAvailability -> bool
// 2. bool -> FindAvailableSinger -> ..., but FindAvailableSinger accepts booking data range, not bool

// Another try:
// 1. BookingRequest -> CheckRoomAvailability -> AvailableRoom
// 2. AvailableRoom -> FindAvailableSinger -> OrganizedRoom
// 3. OrganizedRoom -> Charge -> BookedRoom

// But what about declines? Another try:
// 1. BookingRequest -> CheckRoomAvailability -> AvailableRoom or Declined
// 2. AvailableRoom or Declined -> FindAvailableSinger -> OrganizedRoom or Declined
// 3. OrganizedRoom or Declined -> Charge -> BookedRoom or Declined

type AvailableRoom = {
    RoomNumber: RoomNumber
    BookingDateRange: BookingDateRange
}

type OrganizedRoom = Booking

// For brevity reason
type BookedRoom = OrganizedRoom

// ...Room or Declined
type BookingState<'T> = {
    Room: 'T option
    DeclineReason: DeclineReason option
}

let asBookingResult: BookingState<BookedRoom> -> BookingResult = fun booking ->
    if booking.Room.IsSome then
        let createEvents: SingerParticipation option -> BookingEvent list = fun participation ->
            let events: BookingEvent list = [CustomerNotified { Message = "Welcome to our hotel!" }]
            match participation with
                | None -> events
                | Some participation -> events @ [SingerInvited participation]

        BookingAccepted { DateRange = booking.Room.Value.BookingDateRange; BookingEvents = createEvents booking.Room.Value.Singer }
    else
        BookingDeclined booking.DeclineReason.Value

let bookRoom: BookRoomWorkflow =
    fun checkRoomAvailability findSinger charge bookingRequest ->
        // Note: function adapter
        // Note: function shadowing
        let checkRoomAvailability (booking: BookingRequest): BookingState<AvailableRoom> =
            let available = checkRoomAvailability booking

            if available then
                let room = { RoomNumber = booking.RoomNumber; BookingDateRange = booking.BookingDateRange }
                { Room = Some room; DeclineReason = None }
            else
                { Room = None; DeclineReason = Some RoomUnavailable }

        let findSinger (booking: BookingState<AvailableRoom>): BookingState<OrganizedRoom> =
            if booking.Room.IsSome then
                let availability = findSinger booking.Room.Value.RoomNumber booking.Room.Value.BookingDateRange
                match availability with
                | SingerParticipating participation ->
                    let room = { RoomNumber = booking.Room.Value.RoomNumber; BookingDateRange = booking.Room.Value.BookingDateRange; Singer = Some participation }
                    { Room = Some room; DeclineReason = None }
                | SingerHasNoTime ->
                    { Room = None; DeclineReason = Some SingerUnavailable }
            else
                { Room = None; DeclineReason = booking.DeclineReason }
        let findSinger (booking: BookingState<AvailableRoom>) =
            if bookingRequest.IncludeLuxus then
                findSinger booking
            else
                let room = { RoomNumber = booking.Room.Value.RoomNumber; BookingDateRange = booking.Room.Value.BookingDateRange; Singer = None }
                { Room = Some room; DeclineReason = None }

        let charge (booking: BookingState<OrganizedRoom>): BookingState<BookedRoom> =
            if booking.Room.IsSome then
                let charged = charge booking.Room.Value
                if charged then
                    { Room = Some booking.Room.Value; DeclineReason = None }
                else
                    { Room = None; DeclineReason = Some PaymentDeclined }
            else
                { Room = None; DeclineReason = booking.DeclineReason }
        
        checkRoomAvailability bookingRequest
        |> findSinger
        |> charge
        |> asBookingResult