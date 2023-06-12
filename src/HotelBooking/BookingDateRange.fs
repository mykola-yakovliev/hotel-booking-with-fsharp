module BookingDateRange

open System

type BookingDateRange = {
    StartDate: DateOnly;
    EndDate: DateOnly;
} with override this.ToString() = $"{this.StartDate} - {this.EndDate}"

// Note: easy way to override methods

// Note: make illegal state unrepresentable
let create (startDate: DateOnly, endDate: DateOnly) =
    if startDate > endDate then
        failwith "Start date cannot be greater than end date"
    else
        { StartDate = startDate; EndDate = endDate }
