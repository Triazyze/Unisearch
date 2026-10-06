import { StatusBar } from 'expo-status-bar';
import { Text, View } from 'react-native';
import { Button } from '../../../components/ui/Button';
import { TextLink } from '../../../components/ui/TextLink';
import { useAppNavigation } from '../../../navigation/useAppNavigation';
import { styles } from './AdminMenuScreenStyle';

export default function AdminMenuScreen() {
  const navigation = useAppNavigation();

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <TextLink label="← Retour" onPress={() => navigation.goBack()} style={styles.backLink} />

        <Text style={styles.title}>Administration</Text>

        <View style={styles.actions}>
          <Button
            label="Voir les grilles"
            onPress={() => navigation.navigate('AdminGridList')}
            variant="primary"
          />
          <Button label="Générer une grille" style={styles.nextAction} variant="primary" />
        </View>
      </View>
    </View>
  );
}
