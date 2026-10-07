import { StyleSheet, Text, View, type StyleProp, type ViewStyle } from 'react-native';
import { theme } from '../ui/theme';

type WordBoardProps = {
  foundWordIndexes?: number[];
  style?: StyleProp<ViewStyle>;
  words: string[];
};

export function WordBoard({ foundWordIndexes = [], style, words }: WordBoardProps) {
  const foundWordIndexSet = new Set(foundWordIndexes);

  return (
    <View style={[styles.board, style]}>
      {words.map((word, index) => {
        const isFound = foundWordIndexSet.has(index);

        return (
          <Text key={`${word}-${index}`} style={[styles.word, isFound && styles.foundWord]}>
            {word}
          </Text>
        );
      })}
    </View>
  );
}

const styles = StyleSheet.create({
  board: {
    gap: 24,
  },
  word: {
    color: theme.colors.darkText,
    fontSize: 16,
  },
  foundWord: {
    color: theme.colors.primary,
    textDecorationColor: theme.colors.primary,
    textDecorationLine: 'line-through',
  },
});
