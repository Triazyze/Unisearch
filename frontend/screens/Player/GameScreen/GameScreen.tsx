import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { StatusBar } from 'expo-status-bar';
import { ActivityIndicator, ScrollView, Text, View } from 'react-native';
import { GridBoard } from '../../../components/grid/GridBoard';
import { WordBoard } from '../../../components/grid/WordBoard';
import { AppModal } from '../../../components/ui/AppModal';
import { TextLink } from '../../../components/ui/TextLink';
import { theme } from '../../../components/ui/theme';
import { usePlayerGame } from '../../../hooks/usePlayerGame';
import type { RootStackParamList } from '../../../navigation/RootNavigator';
import { styles } from './GameScreenStyle';

type GameScreenProps = NativeStackScreenProps<RootStackParamList, 'Game'>;

export default function GameScreen({ navigation, route }: GameScreenProps) {
  const {
    completionMessage,
    errorMessage,
    formattedElapsedTime,
    foundWordIndexes,
    grid,
    handleTraceCompleted,
    highlightedCells,
    isCompleted,
    traceAttempts,
  } = usePlayerGame(route.params.gridId);

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
              <Text style={styles.timerText}>◷ {formattedElapsedTime}</Text>
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
                <Text style={styles.metricText}>Nombre d’essais : {traceAttempts}</Text>
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
        visible={isCompleted}
      />
    </View>
  );
}
