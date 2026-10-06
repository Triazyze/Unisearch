import { StatusBar } from 'expo-status-bar';
import { Text, View } from 'react-native';
import { Button } from '../../components/ui/Button';
import { useAppNavigation } from '../../navigation/useAppNavigation';
import { useAuthStore } from '../../stores/useAuthStore';
import { styles } from './MainScreenStyle';

export default function MainScreen() {
  const navigation = useAppNavigation();
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
          <Button
            label={isAdmin ? 'Administrer' : 'Jouer'}
            onPress={() => navigation.navigate(isAdmin ? 'AdminMenu' : 'PlayerMenu')}
            variant="primary"
          />
        </View>

        <Button
          label="Se déconnecter"
          onPress={clearSessionAsync}
          style={styles.logoutButton}
          variant="secondary"
        />
      </View>
    </View>
  );
}
