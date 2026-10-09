import { isAxiosError } from 'axios';
import type { RemotePlayerPack } from '../../types/player/RemotePlayerGrid';
import { apiBaseUrl, apiClient } from '../http/apiClient';

export async function getTestPackAsync(): Promise<RemotePlayerPack> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error(
      "La requête de récupération du pack de test n'a pas réussi à atteindre le serveur.",
    );
  }

  try {
    const response = await apiClient.get<RemotePlayerPack>('/api/packs/player/test');

    return response.data;
  } catch (error) {
    throw new Error(getPackErrorMessage(error));
  }
}

function getPackErrorMessage(error: unknown): string {
  if (isAxiosError<{ message?: unknown }>(error)) {
    const message = error.response?.data?.message;

    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }

  return 'Le pack de test est indisponible pour le moment. Vérifiez votre réseau et réessayez.';
}
