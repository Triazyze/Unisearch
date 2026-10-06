import { create } from 'zustand';
import {
  deleteAccessTokenAsync,
  getAccessTokenAsync,
  saveAccessTokenAsync,
} from '../services/auth/authTokenStorage';
import { createAuthSession, type AuthenticatedUser } from '../services/auth/jwtSession';

type AuthState = {
  isRestoringSession: boolean;
  token: string | null;
  user: AuthenticatedUser | null;
  clearSessionAsync: () => Promise<void>;
  restoreSessionAsync: () => Promise<void>;
  setSessionAsync: (token: string) => Promise<boolean>;
};

export const useAuthStore = create<AuthState>()((set) => ({
  isRestoringSession: true,
  token: null,
  user: null,

  async clearSessionAsync() {
    try {
      await deleteAccessTokenAsync();
    } finally {
      set({ token: null, user: null });
    }
  },

  async restoreSessionAsync() {
    try {
      const token = await getAccessTokenAsync();
      const session = token === null ? null : createAuthSession(token);

      if (session === null) {
        await deleteAccessTokenAsync();
        set({ token: null, user: null });
        return;
      }

      set({ token: session.token, user: session.user });
    } catch {
      set({ token: null, user: null });
    } finally {
      set({ isRestoringSession: false });
    }
  },

  async setSessionAsync(token: string) {
    const session = createAuthSession(token);

    if (session === null) {
      return false;
    }

    await saveAccessTokenAsync(token);
    set({ token: session.token, user: session.user });

    return true;
  },
}));
