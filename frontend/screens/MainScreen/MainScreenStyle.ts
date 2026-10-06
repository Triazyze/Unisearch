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
    paddingTop: 66,
  },
  welcome: {
    color: theme.colors.link,
    fontSize: 15,
  },
  titleContainer: {
    alignItems: 'center',
    height: 171,
    justifyContent: 'center',
  },
  title: {
    color: theme.colors.darkText,
    fontSize: 36,
    fontWeight: '800',
    letterSpacing: 0.5,
  },
  card: {
    borderColor: theme.colors.primary,
    borderRadius: 12,
    borderWidth: 0.5,
    padding: 20,
  },
  cardTitle: {
    color: theme.colors.darkText,
    fontSize: 20,
    fontWeight: '800',
  },
  cardDescription: {
    color: theme.colors.mutedText,
    fontSize: 15,
    marginBottom: 2,
    marginTop: 16,
  },
  logoutButton: {
    alignItems: 'center',
    borderColor: theme.colors.primary,
    borderRadius: theme.radius.button,
    borderWidth: 0.5,
    height: 56,
    justifyContent: 'center',
    marginTop: 'auto',
    marginBottom: 58,
  },
  logoutButtonText: {
    color: theme.colors.primary,
    fontSize: 16,
  },
});
