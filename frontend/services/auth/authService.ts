import { isAxiosError } from 'axios';
import { apiBaseUrl, apiClient } from '../http/apiClient';

type LoginResponse = {
  token: string;
};

type ErrorResponse = {
  message?: unknown;
};

export async function loginAsync(identifier: string, password: string): Promise<string> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("L'adresse de l'API n'est pas configurée.");
  }

  try {
    const response = await apiClient.post<LoginResponse>('/api/auth/login', {
      identifier,
      password,
    });

    return response.data.token;
  } catch (error) {
    throw new Error(getLoginErrorMessage(error));
  }
}

function getLoginErrorMessage(error: unknown): string {
  if (isAxiosError<ErrorResponse>(error)) {
    const message = error.response?.data?.message;

    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }

  return 'La connexion est impossible pour le moment. Vérifiez votre réseau et réessayez.';
}
