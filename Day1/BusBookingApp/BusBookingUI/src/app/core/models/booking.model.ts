export interface Passenger {
    seatNumber: string;
    name: string;
    age: number;
    gender: string;
}

export interface Booking {
    bookingId: string;
    scheduleId: string;
    source: string;
    destination: string;
    departureTime: string;
    totalAmount: number;
    convenienceFee: number;
    status: 'Confirmed' | 'Cancelled' | 'Pending';
    boardingPoint: string;
    dropPoint: string;
    passengers: Passenger[];
}
