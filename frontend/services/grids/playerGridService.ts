import { isAxiosError } from 'axios';
import type { LocalPlayerGrid } from '../../types/player/LocalPlayerGrid';
import { apiBaseUrl, apiClient } from '../http/apiClient';

type GridVerificationRequest = {
  letters: string[];
  time: number;
  traceAttempts: number;
  traces: GridVerificationTrace[];
  words: string[];
};

type GridVerificationTrace = {
  endColumn: number;
  endRow: number;
  startColumn: number;
  startRow: number;
  word: string;
};

export async function verifyPlayerGridAsync(grid: LocalPlayerGrid): Promise<string> {
  if (apiBaseUrl === undefined || apiBaseUrl.length === 0) {
    throw new Error("La requête de validation n'a pas réussi à atteindre le serveur.");
  }

  const request = createGridVerificationRequest(grid);

  try {
    const response = await apiClient.post<{ message: string }>(
      `/api/grids/${grid.gridId}/verification`,
      request,
    );

    return response.data.message;
  } catch (error) {
    throw new Error(getGridVerificationErrorMessage(error));
  }
}

function createGridVerificationRequest(grid: LocalPlayerGrid): GridVerificationRequest {
  return {
    letters: grid.letters,
    time: grid.time,
    traceAttempts: grid.traceAttempts,
    traces: grid.traces.map((trace) => ({
      endColumn: trace.end.column,
      endRow: trace.end.row,
      startColumn: trace.start.column,
      startRow: trace.start.row,
      word: trace.word,
    })),
    words: grid.words,
  };
}

function getGridVerificationErrorMessage(error: unknown): string {
  if (isAxiosError<{ message?: unknown }>(error)) {
    const message = error.response?.data?.message;

    if (typeof message === 'string' && message.length > 0) {
      return message;
    }
  }

  return "La requête de validation n'a pas réussi à atteindre le serveur.";
}
