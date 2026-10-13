import { Gesture, GestureDetector } from 'react-native-gesture-handler';
import Svg, { Line } from 'react-native-svg';
import { useEffect, useReducer, useRef, useState } from 'react';
import { StyleSheet, Text, useWindowDimensions, View } from 'react-native';
import { theme } from '../ui/theme';

export type GridCoordinate = {
  column: number;
  row: number;
};

type GridBoardProps = {
  highlightedCells?: GridCoordinate[];
  letters: string[];
  onTraceCompleted?: (start: GridCoordinate, end: GridCoordinate) => void;
  size: number;
};

type BoardFrame = {
  x: number;
  y: number;
};

type ActiveTrace = {
  end: GridCoordinate;
  start: GridCoordinate;
};

type TraceGestureState = {
  activeTrace: ActiveTrace | null;
  completedTrace: ActiveTrace | null;
};

type TraceGestureAction =
  | { trace: ActiveTrace; type: 'start' }
  | { end: GridCoordinate; type: 'update' }
  | { type: 'complete' }
  | { type: 'clear' }
  | { type: 'consume-completed' };

const cellGap = 8;
const boardWidthRatio = 9 / 10;
const maximumLetterSize = 30;
const minimumLetterSize = 12;
const initialTraceGestureState: TraceGestureState = {
  activeTrace: null,
  completedTrace: null,
};

function traceGestureReducer(
  state: TraceGestureState,
  action: TraceGestureAction,
): TraceGestureState {
  switch (action.type) {
    case 'start':
      return { ...state, activeTrace: action.trace };
    case 'update':
      return state.activeTrace === null
        ? state
        : { ...state, activeTrace: { ...state.activeTrace, end: action.end } };
    case 'complete':
      return { ...state, activeTrace: null, completedTrace: state.activeTrace };
    case 'clear':
      return { ...state, activeTrace: null };
    case 'consume-completed':
      return { ...state, completedTrace: null };
  }
}

export function GridBoard({
  highlightedCells = [],
  letters,
  onTraceCompleted,
  size,
}: GridBoardProps) {
  const { width } = useWindowDimensions();
  const boardWidth = width * boardWidthRatio;
  const cellSize = (boardWidth - cellGap * (size - 1)) / size;
  const letterSize = Math.min(maximumLetterSize, Math.max(minimumLetterSize, cellSize * 0.5));
  const highlightedCellKeys = new Set(highlightedCells.map((cell) => `${cell.row}-${cell.column}`));
  const boardReference = useRef<View>(null);
  const [traceGestureState, dispatchTraceGesture] = useReducer(
    traceGestureReducer,
    initialTraceGestureState,
  );
  const [boardFrame, setBoardFrame] = useState<BoardFrame | null>(null);

  function measureBoardInWindow() {
    boardReference.current?.measureInWindow((x, y) => {
      setBoardFrame({ x, y });
    });
  }

  function getCoordinateAtPoint(absoluteX: number, absoluteY: number): GridCoordinate | null {
    if (boardFrame === null) {
      return null;
    }

    const relativeX = absoluteX - boardFrame.x;
    const relativeY = absoluteY - boardFrame.y;
    const cellAndGapSize = cellSize + cellGap;
    const column = Math.floor(relativeX / cellAndGapSize);
    const row = Math.floor(relativeY / cellAndGapSize);
    const isOutsideBoard = row < 0 || row >= size || column < 0 || column >= size;
    const isInHorizontalGap = relativeX % cellAndGapSize > cellSize;
    const isInVerticalGap = relativeY % cellAndGapSize > cellSize;

    return isOutsideBoard || isInHorizontalGap || isInVerticalGap ? null : { column, row };
  }

  useEffect(() => {
    const completedTrace = traceGestureState.completedTrace;
    if (completedTrace === null) {
      return;
    }

    const isTraceLongEnough =
      completedTrace.start.row !== completedTrace.end.row ||
      completedTrace.start.column !== completedTrace.end.column;
    if (isTraceLongEnough) {
      onTraceCompleted?.(completedTrace.start, completedTrace.end);
    }

    dispatchTraceGesture({ type: 'consume-completed' });
  }, [onTraceCompleted, traceGestureState.completedTrace]);

  const panGesture = Gesture.Pan()
    .runOnJS(true)
    .onBegin((event) => {
      if (onTraceCompleted === undefined) {
        return;
      }

      const coordinate = getCoordinateAtPoint(event.absoluteX, event.absoluteY);
      if (coordinate === null) {
        dispatchTraceGesture({ type: 'clear' });
        return;
      }

      const newActiveTrace = { end: coordinate, start: coordinate };
      dispatchTraceGesture({ trace: newActiveTrace, type: 'start' });
    })
    .onUpdate((event) => {
      const coordinate = getCoordinateAtPoint(event.absoluteX, event.absoluteY);
      if (coordinate === null) {
        return;
      }

      dispatchTraceGesture({ end: coordinate, type: 'update' });
    })
    .onEnd(() => dispatchTraceGesture({ type: 'complete' }))
    .onFinalize(() => dispatchTraceGesture({ type: 'clear' }));

  const activeTraceLine =
    traceGestureState.activeTrace === null
      ? null
      : {
          x1: getCellCenter(traceGestureState.activeTrace.start.column),
          x2: getCellCenter(traceGestureState.activeTrace.end.column),
          y1: getCellCenter(traceGestureState.activeTrace.start.row),
          y2: getCellCenter(traceGestureState.activeTrace.end.row),
        };

  function getCellCenter(index: number): number {
    return index * (cellSize + cellGap) + cellSize / 2;
  }

  const boardContent = (
    <View
      collapsable={false}
      onLayout={measureBoardInWindow}
      ref={boardReference}
      style={[styles.board, { gap: cellGap, height: boardWidth, width: boardWidth }]}
    >
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
                <Text style={[styles.letter, { fontSize: letterSize }]}>{letter}</Text>
              </View>
            );
          })}
        </View>
      ))}

      {activeTraceLine !== null && (
        <Svg
          height={boardWidth}
          pointerEvents="none"
          style={styles.traceOverlay}
          width={boardWidth}
        >
          <Line
            {...activeTraceLine}
            stroke={theme.colors.selection}
            strokeLinecap="round"
            strokeOpacity={0.65}
            strokeWidth={cellSize * 0.55}
          />
        </Svg>
      )}
    </View>
  );

  return onTraceCompleted === undefined ? (
    boardContent
  ) : (
    <GestureDetector gesture={panGesture}>{boardContent}</GestureDetector>
  );
}

const styles = StyleSheet.create({
  board: {
    alignSelf: 'center',
    position: 'relative',
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
  traceOverlay: {
    left: 0,
    position: 'absolute',
    top: 0,
  },
  letter: {
    color: theme.colors.darkText,
    fontWeight: '800',
  },
});
