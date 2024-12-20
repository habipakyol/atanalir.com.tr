export interface User {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    nickname: string | null;
    isEmailVerified: boolean;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface RegisterRequest {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
}

export interface AuthResponse {
    token: string;
}