import { StatusBar } from 'expo-status-bar';
import { Pressable, Text, View } from 'react-native';
import { useAuthStore } from '../../stores/useAuthStore';
import { PrimaryButton } from '../../ui/PrimaryButton';
import { styles } from './MainScreenStyle';

export default function MainScreen() {
  const clearSessionAsync = useAuthStore((state) => state.clearSessionAsync);
  const user = useAuthStore((state) => state.user);

  if (user === null) {
    return null;
  }

  const isAdmin = user.role === 'Admin';

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <Text style={styles.welcome}>Bienvenue, {user.username} !</Text>

        <View style={styles.titleContainer}>
          <Text style={styles.title}>UNISEARCH</Text>
        </View>

        <View style={styles.card}>
          <Text style={styles.cardTitle}>{isAdmin ? 'Administration' : 'Pack de test'}</Text>
          <Text style={styles.cardDescription}>
            {isAdmin ? 'Consultez ou générez des grilles.' : 'Deux grilles disponibles.'}
          </Text>
          <PrimaryButton label={isAdmin ? 'Administrer' : 'Jouer'} />
        </View>

        <Pressable
          accessibilityRole="button"
          onPress={clearSessionAsync}
          style={styles.logoutButton}
        >
          <Text style={styles.logoutButtonText}>Se déconnecter</Text>
        </Pressable>
      </View>
    </View>
  );
}
