import { Modal, StyleSheet, Text, View } from 'react-native';
import { Button } from './Button';
import { theme } from './theme';

type AppModalProps = {
  actionLabel: string;
  message: string;
  onAction: () => void;
  title: string;
  visible: boolean;
};

export function AppModal({ actionLabel, message, onAction, title, visible }: AppModalProps) {
  return (
    <Modal animationType="fade" onRequestClose={onAction} transparent visible={visible}>
      <View style={styles.backdrop}>
        <View accessibilityViewIsModal style={styles.content}>
          <Text accessibilityRole="header" style={styles.title}>
            {title}
          </Text>
          <Text style={styles.message}>{message}</Text>
          <Button label={actionLabel} onPress={onAction} style={styles.action} variant="primary" />
        </View>
      </View>
    </Modal>
  );
}

const styles = StyleSheet.create({
  backdrop: {
    alignItems: 'center',
    backgroundColor: 'rgba(39, 36, 43, 0.42)',
    flex: 1,
    justifyContent: 'center',
    paddingHorizontal: 25,
  },
  content: {
    backgroundColor: theme.colors.white,
    borderRadius: theme.radius.button,
    maxWidth: 420,
    padding: 24,
    width: '100%',
  },
  title: {
    color: theme.colors.darkText,
    fontSize: 22,
    fontWeight: '800',
  },
  message: {
    color: theme.colors.mutedText,
    fontSize: 15,
    lineHeight: 22,
    marginTop: 12,
  },
  action: {
    marginTop: 24,
  },
});
