export interface User {
    userId: number;
    email: string;
    name: string;
    role: 'User' | 'Operator' | 'Admin';
}

export interface DecodedToken {
    userId: string;
    email: string;
    name: string;
    role: 'User' | 'Operator' | 'Admin';
    exp: number;
}
