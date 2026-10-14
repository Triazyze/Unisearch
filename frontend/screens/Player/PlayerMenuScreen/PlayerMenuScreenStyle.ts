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
  pendingGridCard: {
    opacity: 0.55,
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
  pendingStatusBadge: {
    backgroundColor: '#FFF3E0',
    borderColor: '#E88A1C',
  },
  validatedStatusBadge: {
    backgroundColor: '#E9F7EF',
    borderColor: '#2D9D5B',
  },
  statusText: {
    color: theme.colors.darkText,
    fontSize: 12,
  },
  pendingStatusText: {
    color: '#A55300',
  },
  validatedStatusText: {
    color: '#1E7A43',
  },
  gridDescription: {
    color: theme.colors.mutedText,
    fontSize: 15,
    marginTop: 9,
  },
});
