import {
  Text,
  TextInput,
  type TextInputProps,
  View,
  type StyleProp,
  type ViewStyle,
} from 'react-native';
import { theme } from './theme';
import { StyleSheet } from 'react-native';

type TextFieldProps = TextInputProps & {
  containerStyle?: StyleProp<ViewStyle>;
  label: string;
};

export function TextField({ containerStyle, label, style, ...inputProps }: TextFieldProps) {
  return (
    <View style={containerStyle}>
      <Text style={styles.label}>{label}</Text>
      <TextInput
        {...inputProps}
        accessibilityLabel={label}
        placeholderTextColor={styles.placeholder.color}
        style={[styles.input, style]}
      />
    </View>
  );
}

const styles = StyleSheet.create({
  label: {
    color: theme.colors.mutedText,
    fontSize: 13,
    lineHeight: 18,
    marginBottom: 3,
  },
  placeholder: {
    color: theme.colors.mutedText,
  },
  input: {
    borderColor: theme.colors.primary,
    borderRadius: theme.radius.input,
    borderWidth: 0.5,
    color: theme.colors.darkText,
    fontSize: 16,
    height: 56,
    paddingHorizontal: 13,
  },
});
