export interface Seat {
    seatId: string;
    seatNumber: string;
    row: number;
    col: number;   // 1–4 (left pair = 1,2 | right pair = 3,4)
    status: 'Available' | 'Held' | 'Booked';
}
