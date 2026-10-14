import { create } from 'zustand';
import { syncPendingPlayerGridsAsync } from '../services/grids/playerGridService';
import { getTestPackAsync } from '../services/packs/playerPackService';
import {
  readPlayerGridAsync,
  readPlayerPackAsync,
  saveDownloadedPackAsync,
} from '../services/storage/playerGridStorageService';
import type { LocalPlayerGrid } from '../types/player/LocalPlayerGrid';

const testPackName = 'test';

type PlayerGridState = {
  errorMessage: string | null;
  grids: LocalPlayerGrid[] | null;
  isLoading: boolean;
  isSynchronizing: boolean;
  synchronizationMessage: string | null;
  synchronizationMessageId: number;
  clearSynchronizationMessage: () => void;
  loadTestPackAsync: (userId: string) => Promise<void>;
  refreshLocalGridsAsync: (userId: string) => Promise<void>;
  syncPendingGridsAsync: (userId: string, showNotification?: boolean) => Promise<string[]>;
};

export const usePlayerGridStore = create<PlayerGridState>()((set, get) => ({
  errorMessage: null,
  grids: null,
  isLoading: false,
  isSynchronizing: false,
  synchronizationMessage: null,
  synchronizationMessageId: 0,

  clearSynchronizationMessage() {
    set({ synchronizationMessage: null });
  },

  async loadTestPackAsync(userId) {
    set({ errorMessage: null, isLoading: true });

    let downloadErrorMessage: string | null = null;

    try {
      const testPack = await getTestPackAsync();
      await saveDownloadedPackAsync(userId, testPack);
    } catch (error) {
      downloadErrorMessage =
        error instanceof Error ? error.message : 'Le pack de test est indisponible.';
    }

    try {
      await get().refreshLocalGridsAsync(userId);
    } catch (error) {
      set({
        errorMessage:
          downloadErrorMessage ??
          (error instanceof Error ? error.message : 'Le pack de test est indisponible.'),
        grids: null,
      });
    } finally {
      set({ isLoading: false });
    }
  },

  async refreshLocalGridsAsync(userId) {
    const localPack = await readPlayerPackAsync(userId, testPackName);
    if (localPack === null) {
      throw new Error('Le pack de test est introuvable dans le stockage local.');
    }

    const localGrids: LocalPlayerGrid[] = [];
    for (const gridId of localPack.gridIds) {
      const localGrid = await readPlayerGridAsync(userId, gridId);
      if (localGrid === null) {
        throw new Error('Une grille du pack de test est introuvable dans le stockage local.');
      }

      localGrids.push(localGrid);
    }

    set({ errorMessage: null, grids: localGrids });
  },

  async syncPendingGridsAsync(userId, showNotification = false) {
    if (get().isSynchronizing) {
      return [];
    }

    set({ isSynchronizing: true });

    try {
      const validatedGridIds = await syncPendingPlayerGridsAsync(userId);

      if (showNotification && validatedGridIds.length > 0) {
        const synchronizationMessage =
          validatedGridIds.length === 1
            ? '1 grille a été synchronisée.'
            : `${validatedGridIds.length} grilles ont été synchronisées.`;

        set({
          synchronizationMessage,
          synchronizationMessageId: get().synchronizationMessageId + 1,
        });
      }

      await get().refreshLocalGridsAsync(userId);

      return validatedGridIds;
    } finally {
      set({ isSynchronizing: false });
    }
  },
}));
