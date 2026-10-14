import { isAxiosError } from 'axios';
import type { ApiErrorResponse } from '../../types/ApiErrorResponse';

export class ApiRequestError extends Error {
  public constructor(
    message: string,
    public readonly apiErrorResponse: ApiErrorResponse | null = null,
  ) {
    super(message);
    this.name = 'ApiRequestError';
  }
}

export function createApiRequestError(error: unknown, fallbackMessage: string): ApiRequestError {
  if (isAxiosError<ApiErrorResponse>(error) && isApiErrorResponse(error.response?.data)) {
    return new ApiRequestError(error.response.data.message, error.response.data);
  }

  return new ApiRequestError(fallbackMessage);
}

function isApiErrorResponse(value: unknown): value is ApiErrorResponse {
  if (typeof value !== 'object' || value === null) {
    return false;
  }

  const { errorCode, message, statusCode } = value as Record<string, unknown>;

  return (
    typeof errorCode === 'string' && typeof message === 'string' && typeof statusCode === 'number'
  );
}
