import { StatusBar } from 'expo-status-bar';
import { useEffect, useState } from 'react';
import { ActivityIndicator, Pressable, Text, View } from 'react-native';
import { TextLink } from '../../../components/ui/TextLink';
import { useAppNavigation } from '../../../navigation/useAppNavigation';
import { useAuthStore } from '../../../stores/useAuthStore';
import type { LocalPlayerGrid, PlayerGridStatus } from '../../../types/player/LocalPlayerGrid';
import { getTestPackAsync } from '../../../services/packs/playerPackService';
import {
  readPlayerGridAsync,
  readPlayerPackAsync,
  saveDownloadedPackAsync,
} from '../../../services/storage/PlayerGridStorageService';
import { styles } from './PlayerMenuScreenStyle';

const testPackName = 'test';

export default function PlayerMenuScreen() {
  const navigation = useAppNavigation();
  const user = useAuthStore((state) => state.user);
  const [grids, setGrids] = useState<LocalPlayerGrid[] | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    let isActive = true;

    async function loadTestPackAsync() {
      if (user === null) {
        return;
      }

      let downloadErrorMessage: string | null = null;

      try {
        const testPack = await getTestPackAsync();
        await saveDownloadedPackAsync(user.id, testPack);
      } catch (error) {
        downloadErrorMessage =
          error instanceof Error ? error.message : 'Le pack de test est indisponible.';
      }

      try {
        const localPack = await readPlayerPackAsync(user.id, testPackName);
        if (localPack === null) {
          throw new Error(downloadErrorMessage ?? 'Le pack de test est indisponible.');
        }

        const localGrids: LocalPlayerGrid[] = [];
        for (const gridId of localPack.gridIds) {
          const localGrid = await readPlayerGridAsync(user.id, gridId);
          if (localGrid === null) {
            throw new Error('Une grille du pack de test est introuvable dans le stockage local.');
          }

          localGrids.push(localGrid);
        }

        if (isActive) {
          setGrids(localGrids);
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
  }, [user]);

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <TextLink label="← Retour" onPress={() => navigation.goBack()} style={styles.backLink} />

        <Text style={styles.title}>Pack de test</Text>

        {grids === null && errorMessage === null && (
          <ActivityIndicator color="#40227F" size="large" style={styles.loadingIndicator} />
        )}

        {errorMessage !== null && <Text style={styles.errorMessage}>{errorMessage}</Text>}

        {grids !== null && (
          <View style={styles.gridList}>
            {grids.map((grid, index) => (
              <Pressable
                accessibilityLabel={`Jouer la grille ${index + 1}`}
                accessibilityRole="button"
                key={grid.gridId}
                onPress={() =>
                  navigation.navigate('Game', { gridId: grid.gridId, gridNumber: index + 1 })
                }
                style={styles.gridCard}
              >
                <View style={styles.gridHeader}>
                  <Text style={styles.gridTitle}>
                    Grille {index + 1} · {grid.size} × {grid.size}
                  </Text>

                  <View style={styles.statusBadge}>
                    <Text style={styles.statusText}>{getGridStatusLabel(grid.status)}</Text>
                  </View>
                </View>

                <Text style={styles.gridDescription}>
                  {grid.difficulty} · {grid.words.length} {grid.words.length > 1 ? 'mots' : 'mot'}
                </Text>
              </Pressable>
            ))}
          </View>
        )}
      </View>
    </View>
  );
}

function getGridStatusLabel(status: PlayerGridStatus): string {
  switch (status) {
    case 'Pending':
      return 'En attente';
    case 'Validated':
      return 'Validée';
    default:
      return 'À jouer';
  }
}
