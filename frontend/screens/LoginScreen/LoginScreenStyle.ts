import { StyleSheet } from 'react-native';
import { theme } from '../../components/ui/theme';

export const styles = StyleSheet.create({
  screen: {
    flex: 1,
    backgroundColor: theme.colors.white,
  },
  content: {
    flex: 1,
    paddingHorizontal: 30,
  },
  titleContainer: {
    height: 272,
    alignItems: 'center',
    justifyContent: 'center',
  },
  title: {
    color: theme.colors.darkText,
    fontSize: 36,
    fontWeight: '800',
    letterSpacing: 0.5,
  },
  form: {
    width: '100%',
  },
  passwordField: {
    marginTop: 17,
  },
  errorMessage: {
    color: '#B3261E',
    fontSize: 13,
    lineHeight: 18,
    marginTop: 16,
  },
  primaryButton: {
    marginTop: 16,
  },
  accountLink: {
    marginTop: 24,
  },
});
