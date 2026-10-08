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
  updatedAt: string;
  wordCount: number;
};

export type AdminGridDetail = {
  difficulty: string;
  id: string;
  letters: string[];
  pack: {
    date: string | null;
    name: string;
  } | null;
  size: number;
  updatedAt: string;
  words: string[];
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

export async function getGridByIdAsync(gridId: string): Promise<AdminGridDetail> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("L'adresse de l'API n'est pas configurée.");
  }

  try {
    const response = await apiClient.get<AdminGridDetail>(`/api/grids/${gridId}`);

    return response.data;
  } catch (error) {
    throw new Error(getGridDetailErrorMessage(error));
  }
}

export async function createGridAsync(
  size: number,
  words: string[],
  difficulty: string,
): Promise<string> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("L'adresse de l'API n'est pas configurée.");
  }

  try {
    const response = await apiClient.post<{ message: string }>('/api/grids', {
      difficulty,
      size,
      words,
    });

    return response.data.message;
  } catch (error) {
    throw new Error(getGridCreationErrorMessage(error));
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

function getGridDetailErrorMessage(error: unknown): string {
  if (isAxiosError<{ message?: unknown }>(error)) {
    const message = error.response?.data?.message;

    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }

  return 'La grille est indisponible pour le moment. Vérifiez votre réseau et réessayez.';
}

function getGridCreationErrorMessage(error: unknown): string {
  if (isAxiosError<{ message?: unknown }>(error)) {
    const message = error.response?.data?.message;

    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }

  return 'La génération de la grille est impossible pour le moment. Vérifiez votre réseau et réessayez.';
}
