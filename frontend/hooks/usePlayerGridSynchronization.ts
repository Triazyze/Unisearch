import NetInfo, { type NetInfoState } from '@react-native-community/netinfo';
import { useEffect, useRef } from 'react';
import { usePlayerGridStore } from '../stores/usePlayerGridStore';

export function usePlayerGridSynchronization(userId: string | null): void {
  const syncPendingGridsAsync = usePlayerGridStore((state) => state.syncPendingGridsAsync);
  const wasOnlineReference = useRef(false);

  useEffect(() => {
    if (userId === null) {
      return;
    }

    const currentUserId = userId;
    let isActive = true;
    wasOnlineReference.current = false;

    async function synchronizeWhenOnline(networkState: NetInfoState) {
      const isOnline =
        networkState.isConnected === true && networkState.isInternetReachable === true;

      if (!isOnline) {
        wasOnlineReference.current = false;
        return;
      }

      if (wasOnlineReference.current) {
        return;
      }

      wasOnlineReference.current = true;

      try {
        const synchronizationResult = await syncPendingGridsAsync(currentUserId, true);
        wasOnlineReference.current = !synchronizationResult.hasRetryableFailure;
      } catch {
        // L'état est réinitialisé afin que le prochain événement réseau puisse relancer la synchronisation.
        wasOnlineReference.current = false;
      }
    }

    void NetInfo.fetch().then((networkState) => {
      if (isActive) {
        void synchronizeWhenOnline(networkState);
      }
    });

    const unsubscribe = NetInfo.addEventListener((networkState) => {
      if (isActive) {
        void synchronizeWhenOnline(networkState);
      }
    });

    return () => {
      isActive = false;
      unsubscribe();
    };
  }, [syncPendingGridsAsync, userId]);
}
