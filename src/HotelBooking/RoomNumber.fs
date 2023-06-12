// Simple types: pages 79, 104

module RoomNumber

// Note: the hardest part :)
type RoomNumber = private RoomNumber of int
//    ^ type name           ^ case label, also private ctor

let create (roomNumber: int) =
    if (101 <= roomNumber && roomNumber <= 109) || (201 <= roomNumber && roomNumber <= 210) then
        RoomNumber roomNumber
    else
        failwith "Invalid room number"

let value (RoomNumber roomNumber) = roomNumber
