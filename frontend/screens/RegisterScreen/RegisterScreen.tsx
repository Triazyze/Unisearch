import { StatusBar } from 'expo-status-bar';
import { Text, View } from 'react-native';
import { PrimaryButton } from '../../ui/PrimaryButton';
import { TextField } from '../../ui/TextField';
import { TextLink } from '../../ui/TextLink';
import { styles } from './RegisterScreenStyle';
import { useAppNavigation } from '../../navigation/useAppNavigation';

export default function RegisterScreen() {
  const navigation = useAppNavigation();

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <Text style={styles.title}>Créer un compte</Text>

        <View style={styles.form}>
          <TextField autoCapitalize="none" autoComplete="username-new" label="Pseudo" />

          <TextField
            autoCapitalize="none"
            autoComplete="email"
            containerStyle={styles.nextField}
            keyboardType="email-address"
            label="Adresse email"
          />

          <TextField
            autoCapitalize="none"
            autoComplete="new-password"
            containerStyle={styles.nextField}
            label="Mot de passe"
            secureTextEntry
          />

          <Text style={styles.passwordHint}>
            12 caractères minimum, une majuscule,{`\n`}un chiffre et un caractère spécial.
          </Text>

          <PrimaryButton label="Créer mon compte" style={styles.primaryButton} />

          <TextLink onPress={() => navigation.navigate('Login')} label="Déjà un compte ? Se connecter" style={styles.accountLink} />
        </View>
      </View>
    </View>
  );
}
