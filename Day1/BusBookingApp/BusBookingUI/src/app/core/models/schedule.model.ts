export interface Schedule {
    id: number;
    busId: number;
    routeId: number;
    departureTime: string;
    arrivalTime: string;
    pricePerSeat: number;
    status: string;
    boardingPoint: string;
    dropPoint: string;
}
