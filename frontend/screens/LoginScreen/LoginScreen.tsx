import { StatusBar } from 'expo-status-bar';
import { Text, View } from 'react-native';
import { PrimaryButton } from '../../ui/PrimaryButton';
import { TextField } from '../../ui/TextField';
import { TextLink } from '../../ui/TextLink';
import { styles } from './LoginScreenStyle';
import { useAppNavigation } from '../../navigation/useAppNavigation';

export default function LoginScreen() {
  const navigation = useAppNavigation();

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <View style={styles.titleContainer}>
          <Text style={styles.title}>UNISEARCH</Text>
        </View>

        <View style={styles.form}>
          <TextField
            autoCapitalize="none"
            autoComplete="username"
            defaultValue="JoueurCool"
            label="Pseudo ou adresse email"
          />

          <TextField
            autoCapitalize="none"
            autoComplete="current-password"
            containerStyle={styles.passwordField}
            defaultValue="motdepasse12"
            label="Mot de passe"
            secureTextEntry
          />

          <PrimaryButton label="Se connecter" style={styles.primaryButton} />

          <TextLink onPress={() => navigation.navigate('Register')} label="Pas de compte ? Créer un compte" style={styles.accountLink} />
        </View>
      </View>
    </View>
  );
}
