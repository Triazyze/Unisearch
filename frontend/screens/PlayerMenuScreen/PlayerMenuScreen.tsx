import { StatusBar } from 'expo-status-bar';
import { useEffect, useState } from 'react';
import { ActivityIndicator, Text, View } from 'react-native';
import { TextLink } from '../../components/ui/TextLink';
import { useAppNavigation } from '../../navigation/useAppNavigation';
import type { RemotePlayerPack } from '../../types/player/RemotePlayerGrid';
import { getTestPackAsync } from '../../services/packs/playerPackService';
import { styles } from './PlayerMenuScreenStyle';

export default function PlayerMenuScreen() {
  const navigation = useAppNavigation();
  const [pack, setPack] = useState<RemotePlayerPack | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    async function loadTestPackAsync() {
      try {
        const testPack = await getTestPackAsync();

        if (isActive) {
          setPack(testPack);
        }
      } catch (error) {
        if (isActive) {
          setErrorMessage(
            error instanceof Error ? error.message : 'Le pack de test est indisponible.',
          );
        }
      }
    }

    void loadTestPackAsync();

    return () => {
      isActive = false;
    };
  }, []);

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <TextLink label="← Retour" onPress={() => navigation.goBack()} style={styles.backLink} />

        <Text style={styles.title}>Pack de test</Text>

        {pack === null && errorMessage === null && (
          <ActivityIndicator color="#40227F" size="large" style={styles.loadingIndicator} />
        )}

        {errorMessage !== null && <Text style={styles.errorMessage}>{errorMessage}</Text>}

        {pack !== null && (
          <View style={styles.gridList}>
            {pack.grids.map((grid, index) => (
              <View key={grid.id} style={styles.gridCard}>
                <View style={styles.gridHeader}>
                  <Text style={styles.gridTitle}>
                    Grille {index + 1} · {grid.size} × {grid.size}
                  </Text>

                  <View style={styles.statusBadge}>
                    <Text style={styles.statusText}>À jouer</Text>
                  </View>
                </View>

                <Text style={styles.gridDescription}>
                  {grid.difficulty} · {grid.words.length} {grid.words.length > 1 ? 'mots' : 'mot'}
                </Text>
              </View>
            ))}
          </View>
        )}
      </View>
    </View>
  );
}
