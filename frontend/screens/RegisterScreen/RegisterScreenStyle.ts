import { StyleSheet } from 'react-native';
import { theme } from '../../components/ui/theme';

export const styles = StyleSheet.create({
  screen: {
    flex: 1,
    backgroundColor: theme.colors.white,
  },
  content: {
    flex: 1,
    paddingHorizontal: 25,
    paddingTop: 66,
  },
  backButton: {
    alignSelf: 'flex-start',
  },
  backButtonText: {
    color: theme.colors.primary,
    fontSize: 14,
  },
  title: {
    color: theme.colors.darkText,
    fontSize: 26,
    fontWeight: '800',
    marginTop: 25,
  },
  form: {
    marginTop: 27,
  },
  nextField: {
    marginTop: 12,
  },
  passwordHint: {
    color: theme.colors.mutedText,
    fontSize: 12,
    lineHeight: 16,
    marginTop: 7,
  },
  primaryButton: {
    marginTop: 29,
  },
  accountLink: {
    marginTop: 20,
  },
  errorMessage: {
    color: '#B3261E',
    fontSize: 13,
    lineHeight: 18,
    marginTop: 16,
  },
});
