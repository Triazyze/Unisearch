import { StyleSheet } from 'react-native';
import { theme } from '../../../components/ui/theme';

export const styles = StyleSheet.create({
  screen: {
    backgroundColor: theme.colors.white,
    flex: 1,
  },
  content: {
    paddingBottom: 24,
    paddingHorizontal: 25,
    paddingTop: 50,
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
    marginTop: 22,
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
  timerBadge: {
    alignSelf: 'center',
    borderColor: theme.colors.border,
    borderRadius: 20,
    borderWidth: 0.5,
    marginBottom: 24,
    marginTop: 8,
    paddingHorizontal: 14,
    paddingVertical: 10,
  },
  timerText: {
    color: theme.colors.darkText,
    fontSize: 13,
  },
  metrics: {
    flexDirection: 'row',
    gap: 16,
    justifyContent: 'space-between',
    marginTop: 34,
  },
  metricBadge: {
    alignItems: 'center',
    borderColor: theme.colors.border,
    borderRadius: 20,
    borderWidth: 0.5,
    flex: 1,
    paddingHorizontal: 10,
    paddingVertical: 11,
  },
  metricText: {
    color: theme.colors.darkText,
    fontSize: 13,
  },
  wordBoard: {
    marginTop: 28,
  },
});
