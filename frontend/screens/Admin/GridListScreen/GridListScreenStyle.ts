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
  loadingIndicator: {
    marginTop: 50,
  },
  errorMessage: {
    color: '#B3261E',
    fontSize: 14,
    lineHeight: 20,
    marginTop: 35,
  },
  emptyMessage: {
    color: theme.colors.mutedText,
    fontSize: 15,
    marginTop: 35,
  },
  gridList: {
    gap: 16,
    paddingBottom: 30,
    paddingTop: 35,
  },
  gridCard: {
    borderColor: theme.colors.border,
    borderRadius: 12,
    borderWidth: 0.5,
    padding: 16,
  },
  gridTitle: {
    color: theme.colors.darkText,
    fontSize: 19,
    fontWeight: '800',
  },
  gridDescription: {
    color: theme.colors.mutedText,
    fontSize: 15,
    marginTop: 10,
  },
  packDescription: {
    color: theme.colors.primary,
    fontSize: 13,
    marginTop: 8,
  },
});
