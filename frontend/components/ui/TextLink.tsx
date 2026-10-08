import { Pressable, Text, type StyleProp, type TextStyle } from 'react-native';
import { theme } from './theme';
import { StyleSheet } from 'react-native';

type TextLinkProps = {
  label: string;
  onPress?: () => void;
  style?: StyleProp<TextStyle>;
};

export function TextLink({ label, onPress, style }: TextLinkProps) {
  return (
    <Pressable accessibilityLabel={label} accessibilityRole="button" onPress={onPress}>
      <Text style={[styles.label, style]}>{label}</Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  label: {
    color: theme.colors.link,
    fontSize: 13,
    textAlign: 'center',
  },
});
