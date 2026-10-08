import { StyleSheet } from 'react-native';
import { theme } from '../../../components/ui/theme';

export const styles = StyleSheet.create({
  screen: {
    backgroundColor: theme.colors.white,
    flex: 1,
  },
  content: {
    paddingBottom: 40,
    paddingHorizontal: 35,
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
    marginTop: 25,
  },
  form: {
    marginTop: 25,
  },
  nextField: {
    marginTop: 19,
  },
  sizeHint: {
    color: theme.colors.mutedText,
    fontSize: 12,
    marginTop: 7,
  },
  errorMessage: {
    color: '#B3261E',
    fontSize: 13,
    lineHeight: 18,
    marginTop: 16,
  },
  generateButton: {
    marginTop: 32,
  },
});
