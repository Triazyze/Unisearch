import type {
  LocalPlayerGrid,
  LocalPlayerGridTrace,
  PlayerGridCoordinate,
} from '../../types/player/LocalPlayerGrid';

export type PlayerGameState = {
  elapsedSeconds: number;
  foundTraces: LocalPlayerGridTrace[];
  isCompleted: boolean;
  traceAttempts: number;
};

export function createPlayerGameState(grid: LocalPlayerGrid): PlayerGameState {
  const foundTraces = grid.traces;

  return {
    elapsedSeconds: grid.time,
    foundTraces,
    isCompleted: isGameCompleted(grid.words, foundTraces),
    traceAttempts: grid.traceAttempts,
  };
}

export function advancePlayerGame(state: PlayerGameState): PlayerGameState {
  if (state.isCompleted) {
    return state;
  }

  return { ...state, elapsedSeconds: state.elapsedSeconds + 1 };
}

export function applyTraceAttempt(
  grid: LocalPlayerGrid,
  state: PlayerGameState,
  start: PlayerGridCoordinate,
  end: PlayerGridCoordinate,
): PlayerGameState {
  if (state.isCompleted || isSameCoordinate(start, end)) {
    return state;
  }

  const nextState = { ...state, traceAttempts: state.traceAttempts + 1 };
  const foundWordIndexes = getFoundWordIndexes(grid.words, state.foundTraces);
  const traceCoordinates = createTraceCoordinates(start, end);
  const matchingWordIndex = findMatchingWordIndex(
    grid.letters,
    grid.words,
    traceCoordinates,
    foundWordIndexes,
  );

  if (matchingWordIndex === null) {
    return nextState;
  }

  const foundTraces = [
    ...state.foundTraces,
    {
      end,
      start,
      word: grid.words[matchingWordIndex],
    },
  ];

  return {
    ...nextState,
    foundTraces,
    isCompleted: isGameCompleted(grid.words, foundTraces),
  };
}

export function getHighlightedCells(traces: LocalPlayerGridTrace[]): PlayerGridCoordinate[] {
  return traces.flatMap((trace) => createTraceCoordinates(trace.start, trace.end));
}

export function createTraceCoordinates(
  start: PlayerGridCoordinate,
  end: PlayerGridCoordinate,
): PlayerGridCoordinate[] {
  const rowDelta = end.row - start.row;
  const columnDelta = end.column - start.column;

  if (rowDelta !== 0 && columnDelta !== 0 && Math.abs(rowDelta) !== Math.abs(columnDelta)) {
    return [];
  }

  const length = Math.max(Math.abs(rowDelta), Math.abs(columnDelta)) + 1;
  const rowStep = Math.sign(rowDelta);
  const columnStep = Math.sign(columnDelta);

  return Array.from({ length }, (_, index) => ({
    row: start.row + index * rowStep,
    column: start.column + index * columnStep,
  }));
}

export function findMatchingWordIndex(
  letters: string[],
  words: string[],
  traceCoordinates: PlayerGridCoordinate[],
  foundWordIndexes: number[],
): number | null {
  if (traceCoordinates.length < 2) {
    return null;
  }

  const tracedWord = traceCoordinates
    .map((coordinate) => Array.from(letters[coordinate.row] ?? '')[coordinate.column] ?? '')
    .join('');
  const reversedTracedWord = Array.from(tracedWord).reverse().join('');

  const matchingWordIndex = words.findIndex(
    (word, index) =>
      !foundWordIndexes.includes(index) && (word === tracedWord || word === reversedTracedWord),
  );

  return matchingWordIndex === -1 ? null : matchingWordIndex;
}

export function getFoundWordIndexes(words: string[], traces: LocalPlayerGridTrace[]): number[] {
  const foundWordIndexes: number[] = [];

  for (const trace of traces) {
    const wordIndex = words.findIndex(
      (word, index) => word === trace.word && !foundWordIndexes.includes(index),
    );

    if (wordIndex !== -1) {
      foundWordIndexes.push(wordIndex);
    }
  }

  return foundWordIndexes;
}

function isGameCompleted(words: string[], traces: LocalPlayerGridTrace[]): boolean {
  return words.length > 0 && getFoundWordIndexes(words, traces).length === words.length;
}

function isSameCoordinate(start: PlayerGridCoordinate, end: PlayerGridCoordinate): boolean {
  return start.row === end.row && start.column === end.column;
}

export function formatElapsedTime(elapsedSeconds: number): string {
  const minutes = Math.floor(elapsedSeconds / 60);
  const seconds = elapsedSeconds % 60;

  return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
}
