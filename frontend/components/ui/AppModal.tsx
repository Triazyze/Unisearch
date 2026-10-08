import { Modal, StyleSheet, Text, View } from 'react-native';
import { Button } from './Button';
import { theme } from './theme';

export type AppModalOption = {
  label: string;
  value: number | string;
};

type ActionModalProps = {
  actionLabel: string;
  message: string;
  onAction: () => void;
  title: string;
  visible: boolean;
};

type OptionModalProps = {
  message?: string;
  onDismiss: () => void;
  onOptionPress: (value: AppModalOption['value']) => void;
  options: AppModalOption[];
  title: string;
  visible: boolean;
};

type AppModalProps = ActionModalProps | OptionModalProps;

export function AppModal(props: AppModalProps) {
  const isOptionModal = 'options' in props;

  return (
    <Modal
      animationType="fade"
      onRequestClose={isOptionModal ? props.onDismiss : props.onAction}
      transparent
      visible={props.visible}
    >
      <View style={styles.backdrop}>
        <View accessibilityViewIsModal style={styles.content}>
          <Text accessibilityRole="header" style={styles.title}>
            {props.title}
          </Text>
          {props.message !== undefined && <Text style={styles.message}>{props.message}</Text>}

          {isOptionModal ? (
            <View style={styles.options}>
              {props.options.map((option) => (
                <Button
                  key={`${option.value}`}
                  label={option.label}
                  onPress={() => props.onOptionPress(option.value)}
                  variant="secondary"
                />
              ))}
            </View>
          ) : (
            <Button
              label={props.actionLabel}
              onPress={props.onAction}
              style={styles.action}
              variant="primary"
            />
          )}
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
  options: {
    gap: 8,
    marginTop: 24,
  },
});
