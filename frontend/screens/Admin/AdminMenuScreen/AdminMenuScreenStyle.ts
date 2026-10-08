import { StyleSheet } from 'react-native';
import { theme } from '../../../components/ui/theme';

export const styles = StyleSheet.create({
  screen: {
    backgroundColor: theme.colors.white,
    flex: 1,
  },
  content: {
    flex: 1,
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
    marginTop: 25,
  },
  actions: {
    marginTop: 35,
  },
  nextAction: {
    marginTop: 16,
  },
});
