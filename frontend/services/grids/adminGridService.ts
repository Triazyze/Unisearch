import { isAxiosError } from 'axios';
import { apiBaseUrl, apiClient } from '../http/apiClient';

export type AdminGridSummary = {
  difficulty: string;
  id: string;
  pack: {
    date: string | null;
    name: string;
  } | null;
  size: number;
  wordCount: number;
};

export async function getAllGridsAsync(): Promise<AdminGridSummary[]> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("L'adresse de l'API n'est pas configurée.");
  }

  try {
    const response = await apiClient.get<AdminGridSummary[]>('/api/grids');

    return response.data;
  } catch (error) {
    throw new Error(getGridListErrorMessage(error));
  }
}

function getGridListErrorMessage(error: unknown): string {
  if (isAxiosError<{ message?: unknown }>(error)) {
    const message = error.response?.data?.message;

    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }

  return 'Les grilles sont indisponibles pour le moment. Vérifiez votre réseau et réessayez.';
}
