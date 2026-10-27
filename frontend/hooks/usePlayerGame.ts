import { useEffect, useMemo, useRef, useState } from 'react';
import {
  advancePlayerGame,
  applyTraceAttempt,
  createNotPlayedPlayerGrid,
  createPendingPlayerGrid,
  createPlayerGameState,
  formatElapsedTime,
  getFoundWordIndexes,
  getHighlightedCells,
  type PlayerGameState,
} from '../services/game/gridGameService';
import {
  readPlayerGridAsync,
  savePendingGridIdAsync,
  savePlayerGridAsync,
} from '../services/storage/playerGridStorageService';
import { useAuthStore } from '../stores/useAuthStore';
import { usePlayerGridStore } from '../stores/usePlayerGridStore';
import type { LocalPlayerGrid, PlayerGridCoordinate } from '../types/player/LocalPlayerGrid';

type PlayerGame = {
  completionMessage: string;
  errorMessage: string | null;
  formattedElapsedTime: string;
  foundWordIndexes: number[];
  grid: LocalPlayerGrid | null;
  handleTraceCompleted: (start: PlayerGridCoordinate, end: PlayerGridCoordinate) => void;
  highlightedCells: PlayerGridCoordinate[];
  isCompleted: boolean;
  traceAttempts: number;
};

export function usePlayerGame(gridId: string): PlayerGame {
  const user = useAuthStore((state) => state.user);
  const refreshLocalGridsAsync = usePlayerGridStore((state) => state.refreshLocalGridsAsync);
  const syncPendingGridsAsync = usePlayerGridStore((state) => state.syncPendingGridsAsync);
  const [grid, setGrid] = useState<LocalPlayerGrid | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [gameState, setGameState] = useState<PlayerGameState | null>(null);
  const [completionMessage, setCompletionMessage] = useState('Tous les mots ont été trouvés.');
  const savedCompletedGridIdReference = useRef<string | null>(null);
  const foundWordIndexes = useMemo(
    () =>
      grid === null || gameState === null
        ? []
        : getFoundWordIndexes(grid.words, gameState.foundTraces),
    [gameState, grid],
  );
  const highlightedCells = useMemo(
    () => (gameState === null ? [] : getHighlightedCells(gameState.foundTraces)),
    [gameState],
  );

  useEffect(() => {
    let isActive = true;

    async function loadGridAsync() {
      if (user === null) {
        return;
      }

      try {
        const localGrid = await readPlayerGridAsync(user.id, gridId);
        if (localGrid === null) {
          throw new Error('La grille demandée est introuvable dans le stockage local.');
        }

        const playableGrid =
          localGrid.status === 'Validated' ? createNotPlayedPlayerGrid(localGrid) : localGrid;

        if (localGrid.status === 'Validated') {
          await savePlayerGridAsync(user.id, playableGrid);
        }

        if (isActive) {
          savedCompletedGridIdReference.current = null;
          setGrid(playableGrid);
          setErrorMessage(null);
          setGameState(createPlayerGameState(playableGrid));
          setCompletionMessage('Tous les mots ont été trouvés.');
        }
      } catch (error) {
        if (isActive) {
          setErrorMessage(error instanceof Error ? error.message : 'La grille est indisponible.');
        }
      }
    }

    void loadGridAsync();

    return () => {
      isActive = false;
    };
  }, [gridId, user]);

  useEffect(() => {
    if (grid === null || gameState === null || gameState.isCompleted) {
      return;
    }

    const timer = setInterval(() => {
      setGameState((currentGameState) =>
        currentGameState === null ? null : advancePlayerGame(currentGameState),
      );
    }, 1000);

    return () => clearInterval(timer);
  }, [gameState, grid]);

  useEffect(() => {
    if (
      user === null ||
      grid === null ||
      gameState === null ||
      !gameState.isCompleted ||
      savedCompletedGridIdReference.current === grid.gridId
    ) {
      return;
    }

    const completedGrid = grid;
    const completedGameState = gameState;
    const currentUser = user;
    savedCompletedGridIdReference.current = grid.gridId;

    async function saveCompletedGridAsync() {
      const pendingGrid = createPendingPlayerGrid(completedGrid, completedGameState);

      try {
        await savePlayerGridAsync(currentUser.id, pendingGrid);
        await savePendingGridIdAsync(currentUser.id, pendingGrid.gridId);
        setGrid(pendingGrid);
      } catch {
        savedCompletedGridIdReference.current = null;
        setErrorMessage('La grille terminée n’a pas pu être enregistrée localement.');
        return;
      }

      try {
        await refreshLocalGridsAsync(currentUser.id);
      } catch {
        // La partie est déjà enregistrée : un échec de rafraîchissement ne doit pas annuler sa mise en attente.
      }

      try {
        await syncPendingGridsAsync(currentUser.id);
        const synchronizedGrid = await readPlayerGridAsync(currentUser.id, pendingGrid.gridId);

        if (synchronizedGrid?.status === 'Validated') {
          setGrid(synchronizedGrid);
          setCompletionMessage('La grille a été validée avec succès.');
          return;
        }

        if (synchronizedGrid?.status === 'NotPlayed') {
          setGrid(synchronizedGrid);
          setCompletionMessage(
            'La validation a été refusée par le serveur. La grille est de nouveau disponible.',
          );
          return;
        }

        setCompletionMessage('La grille est enregistrée en attente de validation.');
      } catch {
        setCompletionMessage('La grille est enregistrée en attente de validation.');
      }
    }

    void saveCompletedGridAsync();
  }, [gameState, grid, refreshLocalGridsAsync, syncPendingGridsAsync, user]);

  function handleTraceCompleted(start: PlayerGridCoordinate, end: PlayerGridCoordinate) {
    if (grid === null) {
      return;
    }

    setGameState((currentGameState) =>
      currentGameState === null ? null : applyTraceAttempt(grid, currentGameState, start, end),
    );
  }

  return {
    completionMessage,
    errorMessage,
    formattedElapsedTime: formatElapsedTime(gameState?.elapsedSeconds ?? 0),
    foundWordIndexes,
    grid,
    handleTraceCompleted,
    highlightedCells,
    isCompleted: gameState?.isCompleted ?? false,
    traceAttempts: gameState?.traceAttempts ?? 0,
  };
}
