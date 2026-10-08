import { StyleSheet } from 'react-native';
import { theme } from '../../../components/ui/theme';

export const styles = StyleSheet.create({
  screen: {
    backgroundColor: theme.colors.white,
    flex: 1,
  },
  content: {
    paddingBottom: 40,
    paddingHorizontal: 25,
    paddingTop: 66,
  },
  backLink: {
    color: theme.colors.primary,
    fontSize: 14,
    textAlign: 'left',
  },
  title: {
    color: theme.colors.darkText,
    fontSize: 26,
    fontWeight: '800',
    margin: 30,
  },
  loadingIndicator: {
    marginTop: 50,
  },
  errorMessage: {
    color: '#B3261E',
    fontSize: 14,
    lineHeight: 20,
    marginTop: 35,
  },
  metadata: {
    gap: 5,
    marginBottom: 30,
    marginTop: 24,
  },
  metadataText: {
    color: theme.colors.mutedText,
    fontSize: 14,
  },
  wordsTitle: {
    color: theme.colors.darkText,
    fontSize: 20,
    fontWeight: '800',
    marginTop: 36,
  },
  wordBoard: {
    marginTop: 18,
  },
});
