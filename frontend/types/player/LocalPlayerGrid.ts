export type PlayerGridStatus = 'NotPlayed' | 'Pending' | 'Validated';

export type PlayerGridCoordinate = {
  column: number;
  row: number;
};

export type LocalPlayerGridTrace = {
  end: PlayerGridCoordinate;
  start: PlayerGridCoordinate;
  word: string;
};

export type LocalPlayerGrid = {
  difficulty: string;
  gridId: string;
  letters: string[];
  size: number;
  status: PlayerGridStatus;
  time: number;
  traceAttempts: number;
  traces: LocalPlayerGridTrace[];
  updatedAt: string;
  words: string[];
};

export type LocalPlayerPack = {
  gridIds: string[];
  packName: string;
};
