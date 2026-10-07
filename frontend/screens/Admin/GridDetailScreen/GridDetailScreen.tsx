import { StatusBar } from 'expo-status-bar';
import { useEffect, useState } from 'react';
import { ActivityIndicator, ScrollView, Text, View } from 'react-native';
import type { NativeStackScreenProps } from '@react-navigation/native-stack';
import { GridBoard } from '../../../components/grid/GridBoard';
import { WordBoard } from '../../../components/grid/WordBoard';
import { TextLink } from '../../../components/ui/TextLink';
import { theme } from '../../../components/ui/theme';
import type { RootStackParamList } from '../../../navigation/RootNavigator';
import { getGridByIdAsync, type AdminGridDetail } from '../../../services/grids/adminGridService';
import { styles } from './GridDetailScreenStyle';

type GridDetailScreenProps = NativeStackScreenProps<RootStackParamList, 'AdminGridDetail'>;

export default function GridDetailScreen({ navigation, route }: GridDetailScreenProps) {
  const [grid, setGrid] = useState<AdminGridDetail | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    async function loadGridAsync() {
      try {
        const retrievedGrid = await getGridByIdAsync(route.params.gridId);

        if (isActive) {
          setGrid(retrievedGrid);
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
  }, [route.params.gridId]);

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <ScrollView contentContainerStyle={styles.content} showsVerticalScrollIndicator={false}>
        <TextLink label="← Retour" onPress={() => navigation.goBack()} style={styles.backLink} />

        <Text style={styles.title}>Aperçu de la grille</Text>

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
            <GridBoard letters={grid.letters} size={grid.size} />

            <Text style={styles.wordsTitle}>Mots à placer</Text>
            <WordBoard style={styles.wordBoard} words={grid.words} />
          </>
        )}
      </ScrollView>
    </View>
  );
}
