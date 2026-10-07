import { useState } from 'react';
import { Pressable, StyleSheet, Text, View, type StyleProp, type ViewStyle } from 'react-native';
import { AppModal, type AppModalOption } from './AppModal';
import { theme } from './theme';

type SelectFieldProps = {
  containerStyle?: StyleProp<ViewStyle>;
  disabled?: boolean;
  label: string;
  onValueChange: (value: AppModalOption['value']) => void;
  options: AppModalOption[];
  value: AppModalOption['value'];
};

export function SelectField({
  containerStyle,
  disabled = false,
  label,
  onValueChange,
  options,
  value,
}: SelectFieldProps) {
  const [isModalVisible, setIsModalVisible] = useState(false);
  const selectedOption = options.find((option) => option.value === value);

  function handleOptionPress(selectedValue: AppModalOption['value']) {
    onValueChange(selectedValue);
    setIsModalVisible(false);
  }

  return (
    <View style={containerStyle}>
      <Text style={styles.label}>{label}</Text>
      <Pressable
        accessibilityLabel={label}
        accessibilityRole="button"
        disabled={disabled}
        onPress={() => setIsModalVisible(true)}
        style={[styles.field, disabled && styles.disabledField]}
      >
        <Text style={styles.value}>{selectedOption?.label ?? String(value)}</Text>
        <Text accessibilityElementsHidden style={styles.indicator}>
          ⌄
        </Text>
      </Pressable>

      <AppModal
        onDismiss={() => setIsModalVisible(false)}
        onOptionPress={handleOptionPress}
        options={options}
        title={label}
        visible={isModalVisible}
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
  field: {
    alignItems: 'center',
    borderColor: theme.colors.primary,
    borderRadius: theme.radius.input,
    borderWidth: 0.5,
    flexDirection: 'row',
    height: 56,
    justifyContent: 'space-between',
    paddingHorizontal: 13,
  },
  disabledField: {
    opacity: 0.6,
  },
  value: {
    color: theme.colors.darkText,
    fontSize: 16,
  },
  indicator: {
    color: theme.colors.primary,
    fontSize: 20,
  },
});
