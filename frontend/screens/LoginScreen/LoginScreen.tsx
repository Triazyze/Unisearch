import { StatusBar } from 'expo-status-bar';
import { Controller, useForm } from 'react-hook-form';
import { useState } from 'react';
import { Text, View } from 'react-native';
import { Button } from '../../components/ui/Button';
import { TextField } from '../../components/ui/TextField';
import { TextLink } from '../../components/ui/TextLink';
import { styles } from './LoginScreenStyle';
import { useAppNavigation } from '../../navigation/useAppNavigation';
import { loginAsync } from '../../services/auth/authService';
import { useAuthStore } from '../../stores/useAuthStore';

type LoginFormValues = {
  identifier: string;
  password: string;
};

export default function LoginScreen() {
  const navigation = useAppNavigation();
  const setSessionAsync = useAuthStore((state) => state.setSessionAsync);
  const [authenticationErrorMessage, setAuthenticationErrorMessage] = useState<string | null>(null);
  const {
    control,
    formState: { errors, isSubmitting },
    handleSubmit,
  } = useForm<LoginFormValues>({
    defaultValues: {
      identifier: '',
      password: '',
    },
  });

  async function handleLogin({ identifier, password }: LoginFormValues) {
    setAuthenticationErrorMessage(null);

    try {
      const token = await loginAsync(identifier, password);
      const sessionWasCreated = await setSessionAsync(token);

      if (!sessionWasCreated) {
        setAuthenticationErrorMessage('La réponse de connexion est invalide.');
      }
    } catch (error) {
      setAuthenticationErrorMessage(
        error instanceof Error ? error.message : 'La connexion est impossible pour le moment.',
      );
    }
  }

  const formErrorMessage =
    errors.identifier?.message ?? errors.password?.message ?? authenticationErrorMessage;

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <View style={styles.titleContainer}>
          <Text style={styles.title}>UNISEARCH</Text>
        </View>

        <View style={styles.form}>
          <Controller
            control={control}
            name="identifier"
            render={({ field: { onChange, value } }) => (
              <TextField
                autoCapitalize="none"
                autoComplete="username"
                autoCorrect={false}
                editable={!isSubmitting}
                label="Pseudo ou adresse email"
                onChangeText={onChange}
                value={value}
              />
            )}
            rules={{ required: "Le pseudo ou l'adresse email est requis." }}
          />

          <Controller
            control={control}
            name="password"
            render={({ field: { onChange, value } }) => (
              <TextField
                autoCapitalize="none"
                autoComplete="current-password"
                containerStyle={styles.passwordField}
                editable={!isSubmitting}
                label="Mot de passe"
                onChangeText={onChange}
                secureTextEntry
                value={value}
              />
            )}
            rules={{ required: 'Le mot de passe est requis.' }}
          />

          {formErrorMessage !== null && <Text style={styles.errorMessage}>{formErrorMessage}</Text>}

          <Button
            disabled={isSubmitting}
            label={isSubmitting ? 'Connexion...' : 'Se connecter'}
            onPress={handleSubmit(handleLogin)}
            style={styles.primaryButton}
            variant="primary"
          />

          <TextLink
            onPress={() => navigation.navigate('Register')}
            label="Pas de compte ? Créer un compte"
            style={styles.accountLink}
          />
        </View>
      </View>
    </View>
  );
}
