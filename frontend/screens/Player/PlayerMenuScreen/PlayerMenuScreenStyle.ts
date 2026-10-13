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
  gridList: {
    gap: 21,
    marginTop: 35,
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
  gridCard: {
    borderColor: theme.colors.border,
    borderRadius: 12,
    borderWidth: 0.5,
    minHeight: 108,
    padding: 16,
  },
  gridHeader: {
    alignItems: 'center',
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  gridTitle: {
    color: theme.colors.darkText,
    fontSize: 20,
    fontWeight: '800',
  },
  statusBadge: {
    borderColor: theme.colors.border,
    borderRadius: 8,
    borderWidth: 0.5,
    paddingHorizontal: 10,
    paddingVertical: 8,
  },
  statusText: {
    color: theme.colors.darkText,
    fontSize: 12,
  },
  gridDescription: {
    color: theme.colors.mutedText,
    fontSize: 15,
    marginTop: 9,
  },
});
