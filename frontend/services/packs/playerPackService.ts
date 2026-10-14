import type { RemotePlayerPack } from '../../types/player/RemotePlayerGrid';
import { apiBaseUrl, apiClient } from '../http/apiClient';
import { ApiRequestError, createApiRequestError } from '../http/apiRequestError';

export async function getTestPackAsync(): Promise<RemotePlayerPack> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new ApiRequestError(
      "La requête de récupération du pack de test n'a pas réussi à atteindre le serveur.",
    );
  }

  try {
    const response = await apiClient.get<RemotePlayerPack>('/api/packs/player/test');

    return response.data;
  } catch (error) {
    throw createApiRequestError(
      error,
      'Le pack de test est indisponible pour le moment. Vérifiez votre réseau et réessayez.',
    );
  }
}
