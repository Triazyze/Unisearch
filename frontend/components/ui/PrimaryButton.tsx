import { Pressable, Text, type StyleProp, type ViewStyle } from 'react-native';
import { theme } from './theme';
import { StyleSheet } from 'react-native';

type PrimaryButtonProps = {
  disabled?: boolean;
  label: string;
  onPress?: () => void;
  style?: StyleProp<ViewStyle>;
};

export function PrimaryButton({ disabled = false, label, onPress, style }: PrimaryButtonProps) {
  return (
    <Pressable
      accessibilityLabel={label}
      accessibilityRole="button"
      disabled={disabled}
      onPress={onPress}
      style={[styles.button, disabled && styles.disabledButton, style]}
    >
      <Text style={styles.label}>{label}</Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  button: {
    alignItems: 'center',
    backgroundColor: theme.colors.primary,
    borderRadius: theme.radius.button,
    height: 56,
    justifyContent: 'center',
  },
  disabledButton: {
    opacity: 0.6,
  },
  label: {
    color: theme.colors.white,
    fontSize: 16,
  },
});
