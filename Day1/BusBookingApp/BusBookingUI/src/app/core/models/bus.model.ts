export interface BusSearchResult {
    scheduleId: string;
    busName: string;
    operatorName: string;
    source: string;
    destination: string;
    departureTime: string;
    arrivalTime: string;
    pricePerSeat: number;
    totalSeats: number;
    availableSeats: number;
    boardingPoint: string;
    dropPoint: string;
}
