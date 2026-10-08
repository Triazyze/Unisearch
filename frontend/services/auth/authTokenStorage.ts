import * as SecureStore from 'expo-secure-store';

const accessTokenKey = 'unisearch.access-token';

// SecureStore n'est pas disponible sur le web. Le token y reste uniquement en mémoire pour les aperçus locaux.
let webAccessToken: string | null = null;

async function isSecureStoreAvailableAsync(): Promise<boolean> {
  try {
    return await SecureStore.isAvailableAsync();
  } catch {
    return false;
  }
}

export async function getAccessTokenAsync(): Promise<string | null> {
  if (await isSecureStoreAvailableAsync()) {
    return SecureStore.getItemAsync(accessTokenKey);
  }

  return webAccessToken;
}

export async function saveAccessTokenAsync(token: string): Promise<void> {
  if (await isSecureStoreAvailableAsync()) {
    await SecureStore.setItemAsync(accessTokenKey, token);
    return;
  }

  webAccessToken = token;
}

export async function deleteAccessTokenAsync(): Promise<void> {
  if (await isSecureStoreAvailableAsync()) {
    await SecureStore.deleteItemAsync(accessTokenKey);
    return;
  }

  webAccessToken = null;
}
