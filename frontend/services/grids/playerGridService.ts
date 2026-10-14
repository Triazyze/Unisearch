import type { LocalPlayerGrid } from '../../types/player/LocalPlayerGrid';
import { createNotPlayedPlayerGrid } from '../game/gridGameService';
import { apiBaseUrl, apiClient } from '../http/apiClient';
import { ApiRequestError, createApiRequestError } from '../http/apiRequestError';
import {
  readPendingGridIdsAsync,
  readPlayerGridAsync,
  removePendingGridIdAsync,
  savePlayerGridAsync,
} from '../storage/playerGridStorageService';

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
    throw new ApiRequestError("La requête de validation n'a pas réussi à atteindre le serveur.");
  }

  const request = createGridVerificationRequest(grid);

  try {
    const response = await apiClient.post<{ message: string }>(
      `/api/grids/${grid.gridId}/verification`,
      request,
    );

    return response.data.message;
  } catch (error) {
    throw createApiRequestError(
      error,
      "La requête de validation n'a pas réussi à atteindre le serveur.",
    );
  }
}

export async function syncPendingPlayerGridsAsync(userId: string): Promise<string[]> {
  const pendingGridIds = await readPendingGridIdsAsync(userId);
  const validatedGridIds: string[] = [];

  for (const gridId of pendingGridIds) {
    const grid = await readPlayerGridAsync(userId, gridId);
    if (grid === null || grid.status !== 'Pending') {
      continue;
    }

    try {
      await verifyPlayerGridAsync(grid);

      const validatedGrid: LocalPlayerGrid = { ...grid, status: 'Validated' };
      await savePlayerGridAsync(userId, validatedGrid);
      await removePendingGridIdAsync(userId, validatedGrid.gridId);
      validatedGridIds.push(validatedGrid.gridId);
    } catch (error) {
      if (shouldResetPendingGrid(error)) {
        const notPlayedGrid = createNotPlayedPlayerGrid(grid);
        await savePlayerGridAsync(userId, notPlayedGrid);
        await removePendingGridIdAsync(userId, notPlayedGrid.gridId);
        continue;
      }

      // La grille reste en attente afin qu'une prochaine synchronisation puisse la renvoyer.
    }
  }

  return validatedGridIds;
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

function shouldResetPendingGrid(error: unknown): boolean {
  if (!(error instanceof ApiRequestError) || error.apiErrorResponse === null) {
    return false;
  }

  return [
    'BAD_REQUEST',
    'FORBIDDEN',
    'GRID_NOT_FOUND',
    'LETTERS_INVALID',
    'WORDS_INVALID',
    'TRACE_MISMATCH',
    'TRACES_INVALID',
  ].includes(error.apiErrorResponse.errorCode);
}
