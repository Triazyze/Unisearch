import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { StatusBar } from 'expo-status-bar';
import { useEffect, useMemo, useRef, useState } from 'react';
import { ActivityIndicator, ScrollView, Text, View } from 'react-native';
import { GridBoard } from '../../../components/grid/GridBoard';
import { WordBoard } from '../../../components/grid/WordBoard';
import { AppModal } from '../../../components/ui/AppModal';
import { TextLink } from '../../../components/ui/TextLink';
import { theme } from '../../../components/ui/theme';
import type { RootStackParamList } from '../../../navigation/RootNavigator';
import {
  advancePlayerGame,
  applyTraceAttempt,
  createPendingPlayerGrid,
  createPlayerGameState,
  createNotPlayedPlayerGrid,
  formatElapsedTime,
  getFoundWordIndexes,
  getHighlightedCells,
  type PlayerGameState,
} from '../../../services/game/gridGameService';
import { syncPendingPlayerGridsAsync } from '../../../services/grids/playerGridService';
import {
  readPlayerGridAsync,
  savePendingGridIdAsync,
  savePlayerGridAsync,
} from '../../../services/storage/playerGridStorageService';
import { useAuthStore } from '../../../stores/useAuthStore';
import type { LocalPlayerGrid } from '../../../types/player/LocalPlayerGrid';
import { styles } from './GameScreenStyle';

type GameScreenProps = NativeStackScreenProps<RootStackParamList, 'Game'>;

export default function GameScreen({ navigation, route }: GameScreenProps) {
  const user = useAuthStore((state) => state.user);
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
        const localGrid = await readPlayerGridAsync(user.id, route.params.gridId);
        if (localGrid === null) {
          throw new Error('La grille demandée est introuvable dans le stockage local.');
        }

        const playableGrid =
          localGrid.status === 'Validated' ? createNotPlayedPlayerGrid(localGrid) : localGrid;

        if (localGrid.status === 'Validated') {
          await savePlayerGridAsync(user.id, playableGrid);
        }

        if (isActive) {
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
  }, [route.params.gridId, user]);

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
        await syncPendingPlayerGridsAsync(currentUser.id);
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
  }, [gameState, grid, user]);

  function handleTraceCompleted(
    start: { column: number; row: number },
    end: { column: number; row: number },
  ) {
    if (grid === null) {
      return;
    }

    setGameState((currentGameState) =>
      currentGameState === null ? null : applyTraceAttempt(grid, currentGameState, start, end),
    );
  }

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        <TextLink label="← Retour" onPress={() => navigation.goBack()} style={styles.backLink} />

        <Text style={styles.title}>Grille {route.params.gridNumber}</Text>

        {grid === null && errorMessage === null && (
          <ActivityIndicator
            color={theme.colors.primary}
            size="large"
            style={styles.loadingIndicator}
          />
        )}

        {errorMessage !== null && <Text style={styles.errorMessage}>{errorMessage}</Text>}

        {grid !== null && (
          <>
            <View style={styles.timerBadge}>
              <Text style={styles.timerText}>
                ◷ {formatElapsedTime(gameState?.elapsedSeconds ?? 0)}
              </Text>
            </View>

            <GridBoard
              highlightedCells={highlightedCells}
              letters={grid.letters}
              onTraceCompleted={handleTraceCompleted}
              size={grid.size}
            />

            <View style={styles.metrics}>
              <View style={styles.metricBadge}>
                <Text style={styles.metricText}>
                  Mots : {foundWordIndexes.length} / {grid.words.length}
                </Text>
              </View>

              <View style={styles.metricBadge}>
                <Text style={styles.metricText}>
                  Nombre d’essais : {gameState?.traceAttempts ?? 0}
                </Text>
              </View>
            </View>

            <WordBoard
              columnCount={2}
              foundWordIndexes={foundWordIndexes}
              style={styles.wordBoard}
              words={grid.words}
            />
          </>
        )}
      </ScrollView>

      <AppModal
        actionLabel="Continuer"
        message={completionMessage}
        onAction={() => navigation.goBack()}
        title="Grille terminée !"
        visible={gameState?.isCompleted ?? false}
      />
    </View>
  );
}
