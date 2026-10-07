import { StatusBar } from 'expo-status-bar';
import { useEffect, useState } from 'react';
import { ActivityIndicator, Pressable, ScrollView, Text, View } from 'react-native';
import { TextLink } from '../../../components/ui/TextLink';
import { theme } from '../../../components/ui/theme';
import { useAppNavigation } from '../../../navigation/useAppNavigation';
import { getAllGridsAsync, type AdminGridSummary } from '../../../services/grids/adminGridService';
import { styles } from './GridListScreenStyle';

export default function GridListScreen() {
  const navigation = useAppNavigation();
  const [grids, setGrids] = useState<AdminGridSummary[] | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    async function loadGridsAsync() {
      try {
        const retrievedGrids = await getAllGridsAsync();

        if (isActive) {
          setGrids(retrievedGrids);
        }
      } catch (error) {
        if (isActive) {
          setErrorMessage(
            error instanceof Error ? error.message : 'Les grilles sont indisponibles.',
          );
        }
      }
    }

    void loadGridsAsync();

    return () => {
      isActive = false;
    };
  }, []);

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <TextLink label="← Retour" onPress={() => navigation.goBack()} style={styles.backLink} />

        <Text style={styles.title}>Toutes les grilles</Text>

        {grids === null && errorMessage === null && (
          <ActivityIndicator
            color={theme.colors.primary}
            size="large"
            style={styles.loadingIndicator}
          />
        )}

        {errorMessage !== null && <Text style={styles.errorMessage}>{errorMessage}</Text>}

        {grids !== null && grids.length === 0 && (
          <Text style={styles.emptyMessage}>Aucune grille n’a encore été générée.</Text>
        )}

        {grids !== null && grids.length > 0 && (
          <ScrollView contentContainerStyle={styles.gridList} showsVerticalScrollIndicator={false}>
            {grids.map((grid, index) => (
              <Pressable
                accessibilityLabel={`Voir Grille ${index + 1}`}
                accessibilityRole="button"
                key={grid.id}
                onPress={() => navigation.navigate('AdminGridDetail', { gridId: grid.id })}
                style={styles.gridCard}
              >
                <Text style={styles.gridTitle}>
                  Grille {index + 1} · {grid.size} × {grid.size} · {grid.wordCount}{' '}
                  {grid.wordCount > 1 ? 'mots' : 'mot'}
                </Text>
                <Text style={styles.gridDescription}>Difficulté : {grid.difficulty}</Text>
                <Text style={styles.packDescription}>
                  {grid.pack === null ? 'Aucun pack attribué' : `Pack : ${grid.pack.name}`}
                </Text>
              </Pressable>
            ))}
          </ScrollView>
        )}
      </View>
    </View>
  );
}
