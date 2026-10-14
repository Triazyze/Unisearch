import { useFocusEffect } from '@react-navigation/native';
import { StatusBar } from 'expo-status-bar';
import { useCallback } from 'react';
import { ActivityIndicator, Pressable, Text, View } from 'react-native';
import { TextLink } from '../../../components/ui/TextLink';
import { useAppNavigation } from '../../../navigation/useAppNavigation';
import { useAuthStore } from '../../../stores/useAuthStore';
import { usePlayerGridStore } from '../../../stores/usePlayerGridStore';
import type { PlayerGridStatus } from '../../../types/player/LocalPlayerGrid';
import { styles } from './PlayerMenuScreenStyle';

export default function PlayerMenuScreen() {
  const navigation = useAppNavigation();
  const user = useAuthStore((state) => state.user);
  const errorMessage = usePlayerGridStore((state) => state.errorMessage);
  const grids = usePlayerGridStore((state) => state.grids);
  const loadTestPackAsync = usePlayerGridStore((state) => state.loadTestPackAsync);

  useFocusEffect(
    useCallback(() => {
      if (user !== null) {
        void loadTestPackAsync(user.id);
      }
    }, [loadTestPackAsync, user]),
  );

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
            {grids.map((grid, index) => {
              const isPending = grid.status === 'Pending';

              return (
                <Pressable
                  accessibilityLabel={
                    isPending
                      ? `Grille ${index + 1} en attente de validation`
                      : `Jouer la grille ${index + 1}`
                  }
                  accessibilityRole="button"
                  disabled={isPending}
                  key={grid.gridId}
                  onPress={() =>
                    navigation.navigate('Game', { gridId: grid.gridId, gridNumber: index + 1 })
                  }
                  style={[styles.gridCard, isPending && styles.pendingGridCard]}
                >
                  <View style={styles.gridHeader}>
                    <Text style={styles.gridTitle}>
                      Grille {index + 1} · {grid.size} × {grid.size}
                    </Text>

                    <View
                      style={[
                        styles.statusBadge,
                        grid.status === 'Pending' && styles.pendingStatusBadge,
                        grid.status === 'Validated' && styles.validatedStatusBadge,
                      ]}
                    >
                      <Text
                        style={[
                          styles.statusText,
                          grid.status === 'Pending' && styles.pendingStatusText,
                          grid.status === 'Validated' && styles.validatedStatusText,
                        ]}
                      >
                        {getGridStatusLabel(grid.status)}
                      </Text>
                    </View>
                  </View>

                  <Text style={styles.gridDescription}>
                    {grid.difficulty} · {grid.words.length} {grid.words.length > 1 ? 'mots' : 'mot'}
                  </Text>
                </Pressable>
              );
            })}
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
