import { AxiosError } from 'axios';

export interface ApiError {
    message: string;
    statusCode?: number;
}

export type ApiErrorType = AxiosError<ApiError>;