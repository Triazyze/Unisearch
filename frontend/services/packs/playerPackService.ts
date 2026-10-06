import { isAxiosError } from 'axios';
import { apiBaseUrl, apiClient } from '../http/apiClient';

export type PlayerGrid = {
  difficulty: string;
  id: string;
  letters: string[];
  size: number;
  words: string[];
};

export type PlayerPack = {
  grids: PlayerGrid[];
  pack: {
    date: string | null;
    name: string;
  };
};

export async function getTestPackAsync(): Promise<PlayerPack> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("La requête de récupération du pack de test n'a pas réussi à atteindre le serveur.");
  }

  try {
    const response = await apiClient.get<PlayerPack>('/api/packs/player/test');

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
