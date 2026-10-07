import { StatusBar } from 'expo-status-bar';
import { Controller, useForm } from 'react-hook-form';
import { useState } from 'react';
import { ScrollView, Text, View } from 'react-native';
import { AppModal } from '../../../components/ui/AppModal';
import { Button } from '../../../components/ui/Button';
import { SelectField } from '../../../components/ui/SelectField';
import { TextField } from '../../../components/ui/TextField';
import { TextLink } from '../../../components/ui/TextLink';
import { useAppNavigation } from '../../../navigation/useAppNavigation';
import { createGridAsync } from '../../../services/grids/adminGridService';
import { styles } from './GenerateGridScreenStyle';

type GenerateGridFormValues = {
  difficulty: string;
  size: number;
  words: string;
};

const gridSizeOptions = [4, 5, 6, 7, 8, 9, 10].map((size) => ({
  label: `${size}`,
  value: size,
}));

export default function GenerateGridScreen() {
  const navigation = useAppNavigation();
  const [creationErrorMessage, setCreationErrorMessage] = useState<string | null>(null);
  const [creationSuccessMessage, setCreationSuccessMessage] = useState<string | null>(null);
  const {
    control,
    formState: { errors, isSubmitting },
    handleSubmit,
    reset,
  } = useForm<GenerateGridFormValues>({
    defaultValues: {
      difficulty: '',
      size: 6,
      words: '',
    },
  });

  async function handleGenerateGrid({ difficulty, size, words }: GenerateGridFormValues) {
    setCreationErrorMessage(null);

    try {
      const parsedWords = words
        .split(',')
        .map((word) => word.trim())
        .filter((word) => word.length > 0);
      const successMessage = await createGridAsync(size, parsedWords, difficulty);

      reset();
      setCreationSuccessMessage(successMessage);
    } catch (error) {
      setCreationErrorMessage(
        error instanceof Error
          ? error.message
          : 'La génération de la grille est impossible pour le moment.',
      );
    }
  }

  function handleViewGrids() {
    setCreationSuccessMessage(null);
    navigation.replace('AdminGridList');
  }

  const formErrorMessage =
    errors.size?.message ??
    errors.words?.message ??
    errors.difficulty?.message ??
    creationErrorMessage;

  return (
    <View style={styles.screen}>
      <StatusBar style="dark" />

      <ScrollView
        contentContainerStyle={styles.content}
        keyboardShouldPersistTaps="handled"
        showsVerticalScrollIndicator={false}
      >
        <TextLink label="← Retour" onPress={() => navigation.goBack()} style={styles.backLink} />

        <Text style={styles.title}>Générer une grille</Text>

        <View style={styles.form}>
          <Controller
            control={control}
            name="size"
            render={({ field: { onChange, value } }) => (
              <SelectField
                disabled={isSubmitting}
                label="Taille de la grille carrée"
                onValueChange={(selectedSize) => onChange(Number(selectedSize))}
                options={gridSizeOptions}
                value={value}
              />
            )}
          />

          <Text style={styles.sizeHint}>De 4 à 10 cases par côté.</Text>

          <Controller
            control={control}
            name="words"
            render={({ field: { onChange, value } }) => (
              <TextField
                autoCapitalize="characters"
                autoCorrect={false}
                containerStyle={styles.nextField}
                editable={!isSubmitting}
                label="Mots à placer · séparés par des virgules"
                onChangeText={onChange}
                value={value}
              />
            )}
            rules={{
              validate: (value) =>
                value.split(',').some((word) => word.trim().length > 0) ||
                'Au moins un mot est requis.',
            }}
          />

          <Controller
            control={control}
            name="difficulty"
            render={({ field: { onChange, value } }) => (
              <TextField
                autoCapitalize="sentences"
                containerStyle={styles.nextField}
                editable={!isSubmitting}
                label="Difficulté"
                onChangeText={onChange}
                value={value}
              />
            )}
            rules={{ required: 'La difficulté est requise.' }}
          />

          {formErrorMessage !== null && <Text style={styles.errorMessage}>{formErrorMessage}</Text>}

          <Button
            disabled={isSubmitting}
            label={isSubmitting ? 'Génération...' : 'Générer la grille'}
            onPress={handleSubmit(handleGenerateGrid)}
            style={styles.generateButton}
            variant="primary"
          />
        </View>
      </ScrollView>

      <AppModal
        actionLabel="Voir les grilles"
        message={creationSuccessMessage ?? ''}
        onAction={handleViewGrids}
        title="Grille générée"
        visible={creationSuccessMessage !== null}
      />
    </View>
  );
}
