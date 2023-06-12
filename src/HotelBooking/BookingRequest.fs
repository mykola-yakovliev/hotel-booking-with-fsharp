module BookingRequest

open RoomNumber
open BookingDateRange
open PaymentMethod

// Note: had problem due to fsproj file
type BookingRequest = {
    RoomNumber: RoomNumber
    BookingDateRange: BookingDateRange
    IncludeLuxus: bool
    PaymentMethod: PaymentMethod
}
