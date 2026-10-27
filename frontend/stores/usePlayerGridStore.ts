import { create } from 'zustand';
import {
  syncPendingPlayerGridsAsync,
  type PendingGridSynchronizationResult,
} from '../services/grids/playerGridService';
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
  syncPendingGridsAsync: (
    userId: string,
    showNotification?: boolean,
  ) => Promise<PendingGridSynchronizationResult>;
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
    let hasLocalPack = true;

    try {
      await get().refreshLocalGridsAsync(userId);
    } catch {
      // Le premier téléchargement peut ne pas avoir encore créé de pack local.
      hasLocalPack = false;
    }

    try {
      const testPack = await getTestPackAsync();
      await saveDownloadedPackAsync(userId, testPack);
      await get().refreshLocalGridsAsync(userId);
    } catch (error) {
      if (!hasLocalPack) {
        set({
          errorMessage:
            error instanceof Error ? error.message : 'Le pack de test est indisponible.',
        });
      }
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
      return { hasRetryableFailure: false, validatedGridIds: [] };
    }

    set({ isSynchronizing: true });

    try {
      const synchronizationResult = await syncPendingPlayerGridsAsync(userId);

      if (showNotification && synchronizationResult.validatedGridIds.length > 0) {
        const synchronizationMessage =
          synchronizationResult.validatedGridIds.length === 1
            ? '1 grille a été synchronisée.'
            : `${synchronizationResult.validatedGridIds.length} grilles ont été synchronisées.`;

        set({
          synchronizationMessage,
          synchronizationMessageId: get().synchronizationMessageId + 1,
        });
      }

      await get().refreshLocalGridsAsync(userId);

      return synchronizationResult;
    } finally {
      set({ isSynchronizing: false });
    }
  },
}));
