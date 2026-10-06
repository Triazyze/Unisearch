import { Pressable, StyleSheet, Text, type StyleProp, type ViewStyle } from 'react-native';
import { theme } from './theme';

type ButtonVariant = 'primary' | 'secondary';

type ButtonProps = {
  disabled?: boolean;
  label: string;
  onPress?: () => void;
  style?: StyleProp<ViewStyle>;
  variant: ButtonVariant;
};

export function Button({ disabled = false, label, onPress, style, variant }: ButtonProps) {
  const isPrimary = variant === 'primary';

  return (
    <Pressable
      accessibilityLabel={label}
      accessibilityRole="button"
      disabled={disabled}
      onPress={onPress}
      style={[
        styles.button,
        isPrimary ? styles.primaryButton : styles.secondaryButton,
        disabled && styles.disabledButton,
        style,
      ]}
    >
      <Text style={[styles.label, isPrimary ? styles.primaryLabel : styles.secondaryLabel]}>
        {label}
      </Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  button: {
    alignItems: 'center',
    borderRadius: theme.radius.button,
    height: 56,
    justifyContent: 'center',
  },
  primaryButton: {
    backgroundColor: theme.colors.primary,
  },
  secondaryButton: {
    borderColor: theme.colors.border,
    borderWidth: 0.5,
  },
  disabledButton: {
    opacity: 0.6,
  },
  label: {
    fontSize: 16,
  },
  primaryLabel: {
    color: theme.colors.white,
  },
  secondaryLabel: {
    color: theme.colors.primary,
  },
});
