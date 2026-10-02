import { StyleSheet } from 'react-native';
import { colors } from '../../constants/colors';

export const styles = StyleSheet.create({
  screen: {
    flex: 1,
    backgroundColor: colors.white,
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
    color: colors.darkText,
    fontSize: 36,
    fontWeight: '800',
    letterSpacing: 0.5,
  },
  form: {
    width: '100%',
  },
  label: {
    color: colors.mutedText,
    fontSize: 13,
    lineHeight: 18,
    marginBottom: 3,
  },
  placeholder: {
    color: colors.mutedText,
  },
  passwordLabel: {
    marginTop: 17,
  },
  input: {
    borderColor: colors.primary,
    borderRadius: 5,
    borderWidth: 0.5,
    color: colors.darkText,
    fontSize: 16,
    height: 56,
    paddingHorizontal: 13,
  },
  primaryButton: {
    alignItems: 'center',
    backgroundColor: colors.primary,
    borderRadius: 13,
    height: 56,
    justifyContent: 'center',
    marginTop: 32,
  },
  primaryButtonText: {
    color: colors.white,
    fontSize: 16,
  },
  accountLink: {
    color: '#5531B0',
    fontSize: 13,
    marginTop: 24,
    textAlign: 'center',
  },
});
