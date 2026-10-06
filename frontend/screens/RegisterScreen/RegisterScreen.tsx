import { StatusBar } from 'expo-status-bar';
import { Text, View } from 'react-native';
import { Controller, useForm } from 'react-hook-form';
import { useState } from 'react';
import { useAppNavigation } from '../../navigation/useAppNavigation';
import { registerAsync } from '../../services/auth/authService';
import { AppModal } from '../../components/ui/AppModal';
import { Button } from '../../components/ui/Button';
import { TextField } from '../../components/ui/TextField';
import { TextLink } from '../../components/ui/TextLink';
import { styles } from './RegisterScreenStyle';

type RegisterFormValues = {
  username: string;
  email: string;
  password: string;
};

export default function RegisterScreen() {
  const navigation = useAppNavigation();
  const [registrationErrorMessage, setRegistrationErrorMessage] = useState<string | null>(null);
  const [registrationSuccessMessage, setRegistrationSuccessMessage] = useState<string | null>(null);

  const {
    control,
    formState: { errors, isSubmitting },
    handleSubmit,
    reset,
  } = useForm<RegisterFormValues>({
    defaultValues: {
      username: '',
      email: '',
      password: '',
    },
  });

  async function handleRegister({ username, email, password }: RegisterFormValues) {
    setRegistrationErrorMessage(null);

    try {
      const successMessage = await registerAsync(username, email, password);

      reset();
      setRegistrationSuccessMessage(successMessage);
    } catch (error) {
      setRegistrationErrorMessage(
        error instanceof Error ? error.message : "L'inscription est impossible pour le moment.",
      );
    }
  }

  const formErrorMessage =
    errors.username?.message ??
    errors.email?.message ??
    errors.password?.message ??
    registrationErrorMessage;

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <View style={styles.content}>
        <Text style={styles.title}>Créer un compte</Text>

        <View style={styles.form}>
          <Controller
            control={control}
            name="username"
            rules={{ required: 'Le pseudo est requis.' }}
            render={({ field: { onChange, value } }) => (
              <TextField
                autoCapitalize="none"
                autoComplete="username-new"
                editable={!isSubmitting}
                label="Pseudo"
                onChangeText={onChange}
                value={value}
              />
            )}
          />

          <Controller
            control={control}
            name="email"
            rules={{
              required: "L'adresse email est requise.",
              pattern: {
                value: /^\S+@\S+\.\S+$/,
                message: "L'adresse email est invalide.",
              },
            }}
            render={({ field: { onChange, value } }) => (
              <TextField
                autoCapitalize="none"
                autoComplete="email"
                editable={!isSubmitting}
                label="Adresse email"
                onChangeText={onChange}
                value={value}
              />
            )}
          />

          <Controller
            control={control}
            name="password"
            rules={{
              required: 'Le mot de passe est requis.',
              minLength: {
                value: 12,
                message: 'Le mot de passe doit contenir au moins 12 caractères.',
              },
              pattern: {
                value: /^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).+$/,
                message:
                  'Le mot de passe doit contenir une majuscule, un chiffre et un caractère spécial.',
              },
            }}
            render={({ field: { onChange, value } }) => (
              <TextField
                autoCapitalize="none"
                autoComplete="password"
                editable={!isSubmitting}
                label="Mot de passe"
                secureTextEntry
                onChangeText={onChange}
                value={value}
              />
            )}
          />

          <Text style={styles.passwordHint}>
            12 caractères minimum, une majuscule,{`\n`}un chiffre et un caractère spécial.
          </Text>

          {formErrorMessage !== null && <Text style={styles.errorMessage}>{formErrorMessage}</Text>}

          <Button
            disabled={isSubmitting}
            label={isSubmitting ? 'Création...' : 'Créer mon compte'}
            onPress={handleSubmit(handleRegister)}
            style={styles.primaryButton}
            variant="primary"
          />

          <TextLink
            onPress={() => navigation.navigate('Login')}
            label="Déjà un compte ? Se connecter"
            style={styles.accountLink}
          />
        </View>
      </View>

      <AppModal
        actionLabel="Se connecter"
        message={registrationSuccessMessage ?? ''}
        onAction={() => navigation.replace('Login')}
        title="Compte créé"
        visible={registrationSuccessMessage !== null}
      />
    </View>
  );
}
