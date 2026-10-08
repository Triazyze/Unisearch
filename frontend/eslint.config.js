// Configuration ESLint pour Expo SDK 57 et React Native.
const { defineConfig, globalIgnores } = require('eslint/config');
const expoConfig = require('eslint-config-expo/flat');
const prettierConfig = require('eslint-config-prettier');

module.exports = defineConfig([
  // Les dossiers générés ne font pas partie du code source à analyser.
  globalIgnores(['.expo/**', 'dist/**', 'web-build/**', 'coverage/**']),

  // Règles recommandées par Expo, incluant React et React Native.
  expoConfig,

  // Désactive les règles ESLint qui entreraient en conflit avec Prettier.
  prettierConfig,

  {
    rules: {
      // Utile en développement, mais à éviter dans le code livré : avertissement, pas blocage.
      'no-console': 'warn',
      // Faux positif avec l'import par défaut officiel d'Axios.
      'import/no-named-as-default-member': 'off',
    },
  },
]);
