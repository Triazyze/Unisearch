import AsyncStorage from '@react-native-async-storage/async-storage';
import type { RemotePlayerGrid, RemotePlayerPack } from '../../types/player/RemotePlayerGrid';
import type { LocalPlayerGrid, LocalPlayerPack } from '../../types/player/LocalPlayerGrid';


// Fonctions pour lire et sauvegarder les packs et grilles de joueurs dans le stockage local
export async function saveDownloadedPackAsync(
  userId: string,
  downloadedPack: RemotePlayerPack,
): Promise<void> {
  for (const downloadedGrid of downloadedPack.grids) {
    await saveDownloadedGridAsync(userId, downloadedGrid);
  }

  const localPack: LocalPlayerPack = {
    packName: downloadedPack.pack.name,
    gridIds: downloadedPack.grids.map((grid) => grid.id),
  };

  await AsyncStorage.setItem(
    getPackStorageKey(userId, localPack.packName),
    JSON.stringify(localPack),
  );
}

export async function readPlayerPackAsync(userId: string,packName: string): Promise<LocalPlayerPack | null> {
  const pack = await AsyncStorage.getItem(getPackStorageKey(userId, packName));
  if (pack === null) {
    return null;
  }
  return JSON.parse(pack) as LocalPlayerPack;
}

export async function saveDownloadedGridAsync(userId: string,downloadedGrid: RemotePlayerGrid): Promise<void> {
  const localGrid: LocalPlayerGrid = {
    gridId: downloadedGrid.id,
    size: downloadedGrid.size,
    letters: downloadedGrid.letters,
    words: downloadedGrid.words,
    difficulty: downloadedGrid.difficulty,
    updatedAt: downloadedGrid.updatedAt,
    status: 'NotPlayed',
    time: 0,
    traceAttempts: 0,
    traces: [],
  };

  await AsyncStorage.setItem(
    getGridStorageKey(userId, localGrid.gridId),
    JSON.stringify(localGrid),
  );
}

export async function readPlayerGridAsync(userId: string,gridId: string): Promise<LocalPlayerGrid | null> {
  const grid = await AsyncStorage.getItem(getGridStorageKey(userId, gridId));
  if (grid === null) {
    return null;
  }
  return JSON.parse(grid) as LocalPlayerGrid;
}

export async function savePlayerGridAsync(userId: string, grid: LocalPlayerGrid): Promise<void> {
  await AsyncStorage.setItem(
    getGridStorageKey(userId, grid.gridId),
    JSON.stringify(grid),
  );
}

// Fonctions pour gérer les grilles en attente de validation
export async function readPendingGridIdsAsync(userId: string): Promise<string[]> {
  const pendingIds = await AsyncStorage.getItem(getPendingStorageKey(userId));
  if (pendingIds === null) {
    return [];
  }
  return JSON.parse(pendingIds) as string[];
}

export async function savePendingGridIdAsync(userId: string, gridId: string): Promise<void> {
  const pendingIds = await readPendingGridIdsAsync(userId);
  if (!pendingIds.includes(gridId)) {
    pendingIds.push(gridId);
    await AsyncStorage.setItem(getPendingStorageKey(userId), JSON.stringify(pendingIds));
  }
}

export async function removePendingGridIdAsync(userId: string, gridId: string): Promise<void> {
  const pendingIds = await readPendingGridIdsAsync(userId);
  const index = pendingIds.indexOf(gridId);
  if (index !== -1) {
    pendingIds.splice(index, 1);
    await AsyncStorage.setItem(getPendingStorageKey(userId), JSON.stringify(pendingIds));
  }
}

// Fonctions pour générer les clés de stockage
function getPackStorageKey(userId: string, packName: string): string {
  return `unisearch:${userId}:pack:${packName}`;
}

function getGridStorageKey(userId: string, gridId: string): string {
  return `unisearch:${userId}:grid:${gridId}`;
}

function getPendingStorageKey(userId: string): string {
  return `unisearch:${userId}:pending`;
}
