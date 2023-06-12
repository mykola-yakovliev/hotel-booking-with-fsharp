// Note: used sources
// - https://fsharpforfunandprofit.com/
// - Domain Modeling Made Functional by Scott Wlaschin
open System
open PaymentMethod

let roomNumber = RoomNumber.create(201)
printfn $"Room number is {RoomNumber.value(roomNumber)}"

let startDate = DateOnly.FromDateTime(DateTime.UtcNow)
let endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7))
let bookingDateRange = BookingDateRange.create(startDate, endDate)
printfn $"Date range: {bookingDateRange}"

let paymentMethod: PaymentMethod = Cash
printfn $"Payment: {paymentMethod}"

// Note: forgot to include luxus - compilation error
let bookingRequest: BookingRequest.BookingRequest = {
    RoomNumber = roomNumber;
    BookingDateRange = bookingDateRange;
    PaymentMethod = paymentMethod;
    IncludeLuxus = true;
}
printfn $"Booking request: {bookingRequest}"

let checkRoomAvailability: RoomBooking.CheckRoomAvailability = fun bookingRequest ->
    true
    //false
let findSinger: Singer.FindAvailableSinger = fun roomNumber dateRange ->
    //Singer.SingerParticipating { Name = Singer.SingerName "Michael Jackson"; Price = Singer.SingerPrice 100500; Date = startDate }
    Singer.SingerHasNoTime
let charge: RoomBooking.Charge = fun booking ->
    true
    //false

// Note: Partial application
let bookRoom = RoomBooking.bookRoom checkRoomAvailability findSinger charge

// Note: deterministic function (no side effects)
let bookingResult = bookRoom bookingRequest
printfn ""
printfn ""
printfn $"Booking result: {bookingResult}"


// Note: built-in equality comparison
//let room1 = RoomNumber.create(202)
//let room2 = RoomNumber.create(202)
//printfn $"Room numbers are equal: {room1 = room2}"
//let bookingDateRange1 = BookingDateRange.create(startDate, endDate)
//let bookingDateRange2 = BookingDateRange.create(startDate, endDate)
//printfn $"Date ranges are equal: {bookingDateRange1 = bookingDateRange2}"
