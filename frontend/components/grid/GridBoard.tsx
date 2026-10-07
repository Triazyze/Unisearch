import { StyleSheet, Text, useWindowDimensions, View } from 'react-native';
import { theme } from '../ui/theme';

export type GridCoordinate = {
  column: number;
  row: number;
};

type GridBoardProps = {
  highlightedCells?: GridCoordinate[];
  letters: string[];
  size: number;
};

const cellGap = 8;
const maximumCellSize = 35;
const horizontalScreenPadding = 50;

export function GridBoard({ highlightedCells = [], letters, size }: GridBoardProps) {
  const { width } = useWindowDimensions();
  const maximumBoardWidth = width - horizontalScreenPadding;
  const cellSize = Math.min(maximumCellSize, (maximumBoardWidth - cellGap * (size - 1)) / size);
  const highlightedCellKeys = new Set(highlightedCells.map((cell) => `${cell.row}-${cell.column}`));

  return (
    <View style={[styles.board, { gap: cellGap, width: cellSize * size + cellGap * (size - 1) }]}>
      {letters.map((row, rowIndex) => (
        <View key={`${row}-${rowIndex}`} style={[styles.row, { gap: cellGap }]}>
          {Array.from(row).map((letter, columnIndex) => {
            const isHighlighted = highlightedCellKeys.has(`${rowIndex}-${columnIndex}`);

            return (
              <View
                key={`${letter}-${columnIndex}`}
                style={[
                  styles.cell,
                  { height: cellSize, width: cellSize },
                  isHighlighted && styles.highlightedCell,
                ]}
              >
                <Text style={styles.letter}>{letter}</Text>
              </View>
            );
          })}
        </View>
      ))}
    </View>
  );
}

const styles = StyleSheet.create({
  board: {
    alignSelf: 'center',
  },
  row: {
    flexDirection: 'row',
  },
  cell: {
    alignItems: 'center',
    borderRadius: 14,
    justifyContent: 'center',
  },
  highlightedCell: {
    backgroundColor: theme.colors.selection,
  },
  letter: {
    color: theme.colors.darkText,
    fontSize: 18,
    fontWeight: '800',
  },
});
