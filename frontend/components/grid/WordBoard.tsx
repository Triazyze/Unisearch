import { StyleSheet, Text, View, type StyleProp, type ViewStyle } from 'react-native';
import { theme } from '../ui/theme';

type WordBoardProps = {
  columnCount?: 1 | 2;
  foundWordIndexes?: number[];
  style?: StyleProp<ViewStyle>;
  words: string[];
};

export function WordBoard({
  columnCount = 1,
  foundWordIndexes = [],
  style,
  words,
}: WordBoardProps) {
  const foundWordIndexSet = new Set(foundWordIndexes);
  const usesTwoColumns = columnCount === 2;

  return (
    <View style={[styles.board, usesTwoColumns && styles.twoColumnBoard, style]}>
      {words.map((word, index) => {
        const isFound = foundWordIndexSet.has(index);

        return (
          <View key={`${word}-${index}`} style={usesTwoColumns ? styles.twoColumnWord : undefined}>
            <Text style={[styles.word, isFound && styles.foundWord]}>{word}</Text>
          </View>
        );
      })}
    </View>
  );
}

const styles = StyleSheet.create({
  board: {
    gap: 8,
  },
  twoColumnBoard: {
    columnGap: 0,
    flexDirection: 'row',
    flexWrap: 'wrap',
    rowGap: 8,
  },
  twoColumnWord: {
    width: '50%',
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
