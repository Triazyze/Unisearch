import { StatusBar } from 'expo-status-bar';
import { Text, TextInput, View } from 'react-native';
import { styles } from './LoginScreenStyle';

export default function LoginScreen() {
  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <View style={styles.titleContainer}>
          <Text style={styles.title}>UNISEARCH</Text>
        </View>

        <View style={styles.form}>
          <Text style={styles.label}>Pseudo ou adresse email</Text>
          <TextInput
            accessibilityLabel="Pseudo ou adresse email"
            autoCapitalize="none"
            autoComplete="username"
            defaultValue="JoueurCool"
            placeholderTextColor={styles.placeholder.color}
            style={styles.input}
          />

          <Text style={[styles.label, styles.passwordLabel]}>Mot de passe</Text>
          <TextInput
            accessibilityLabel="Mot de passe"
            autoCapitalize="none"
            autoComplete="current-password"
            defaultValue="motdepasse12"
            secureTextEntry
            style={styles.input}
          />

          <View style={styles.primaryButton}>
            <Text style={styles.primaryButtonText}>Se connecter</Text>
          </View>

          <Text style={styles.accountLink}>Pas de compte ? Créer un compte</Text>
        </View>
      </View>
    </View>
  );
}
