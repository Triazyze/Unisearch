import { isAxiosError } from 'axios';
import { apiBaseUrl, apiClient } from '../http/apiClient';

export async function loginAsync(identifier: string, password: string): Promise<string> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("La requête de connexion n'a pas réussi à atteindre le serveur.");
  }
  try {
    const response = await apiClient.post<{ token: string }>('/api/auth/login', {
      identifier,
      password,
    });
    return response.data.token;
  } catch (error) {
    throw new Error(
      getAuthErrorMessage(
        error,
        'La connexion est impossible pour le moment. Vérifiez votre réseau et réessayez.',
      ),
    );
  }
}

function getAuthErrorMessage(error: unknown, fallbackMessage: string): string {
  if (isAxiosError<{ message?: unknown }>(error)) {
    const message = error.response?.data?.message;

    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }
  return fallbackMessage;
}

export async function registerAsync(username: string, email: string, password: string ): Promise<string> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("La requête d'inscription n'a pas réussi à atteindre le serveur.");
  }
  try {
    const response = await apiClient.post<{ message: string }>('/api/auth/register', {
      username,
      email,
      password,
    });
    return response.data.message;
  } catch (error) {
    throw new Error(
      getAuthErrorMessage(
        error,
        "L'inscription est impossible pour le moment. Vérifiez votre réseau et réessayez.",
      ),
    );
  }
}
