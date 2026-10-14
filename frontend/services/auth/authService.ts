import { apiBaseUrl, apiClient } from '../http/apiClient';
import { ApiRequestError, createApiRequestError } from '../http/apiRequestError';

export async function loginAsync(identifier: string, password: string): Promise<string> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new ApiRequestError("La requête de connexion n'a pas réussi à atteindre le serveur.");
  }
  try {
    const response = await apiClient.post<{ token: string }>('/api/auth/login', {
      identifier,
      password,
    });
    return response.data.token;
  } catch (error) {
    throw createApiRequestError(
      error,
      'La connexion est impossible pour le moment. Vérifiez votre réseau et réessayez.',
    );
  }
}

export async function registerAsync(
  username: string,
  email: string,
  password: string,
): Promise<string> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new ApiRequestError("La requête d'inscription n'a pas réussi à atteindre le serveur.");
  }
  try {
    const response = await apiClient.post<{ message: string }>('/api/auth/register', {
      username,
      email,
      password,
    });
    return response.data.message;
  } catch (error) {
    throw createApiRequestError(
      error,
      "L'inscription est impossible pour le moment. Vérifiez votre réseau et réessayez.",
    );
  }
}
