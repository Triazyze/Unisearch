export type RemotePlayerGrid = {
  difficulty: string;
  id: string;
  letters: string[];
  size: number;
  updatedAt: string;
  words: string[];
};

export type RemotePlayerPack = {
  grids: RemotePlayerGrid[];
  pack: {
    date: string | null;
    name: string;
  };
};
