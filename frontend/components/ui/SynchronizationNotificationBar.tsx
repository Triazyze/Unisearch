import { useEffect } from 'react';
import { StyleSheet, Text, View } from 'react-native';
import { usePlayerGridStore } from '../../stores/usePlayerGridStore';

const notificationDurationMilliseconds = 5000;

export function SynchronizationNotificationBar() {
  const clearSynchronizationMessage = usePlayerGridStore(
    (state) => state.clearSynchronizationMessage,
  );
  const synchronizationMessage = usePlayerGridStore((state) => state.synchronizationMessage);
  const synchronizationMessageId = usePlayerGridStore((state) => state.synchronizationMessageId);

  useEffect(() => {
    if (synchronizationMessage === null) {
      return;
    }

    const timeout = setTimeout(clearSynchronizationMessage, notificationDurationMilliseconds);

    return () => clearTimeout(timeout);
  }, [clearSynchronizationMessage, synchronizationMessage, synchronizationMessageId]);

  if (synchronizationMessage === null) {
    return null;
  }

  return (
    <View accessibilityLiveRegion="polite" style={styles.container}>
      <Text style={styles.message}>{synchronizationMessage}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    backgroundColor: '#1E7A43',
    borderRadius: 10,
    elevation: 10,
    left: 24,
    paddingHorizontal: 16,
    paddingVertical: 12,
    position: 'absolute',
    right: 24,
    top: 8,
    zIndex: 10,
  },
  message: {
    color: '#FFFFFF',
    fontSize: 14,
    fontWeight: '700',
    textAlign: 'center',
  },
});
