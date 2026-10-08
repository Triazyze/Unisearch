export type UserRole = 'Player' | 'Admin';

export type AuthenticatedUser = {
  id: string;
  role: UserRole;
  username: string;
};

export type AuthSession = {
  token: string;
  user: AuthenticatedUser;
};

type JwtPayload = {
  exp?: unknown;
  name?: unknown;
  role?: unknown;
  sub?: unknown;
};

export function createAuthSession(token: string): AuthSession | null {
  const payload = decodeJwtPayload(token);

  if (
    payload === null ||
    typeof payload.sub !== 'string' ||
    typeof payload.name !== 'string' ||
    !isUserRole(payload.role) ||
    typeof payload.exp !== 'number' ||
    isExpired(payload.exp)
  ) {
    return null;
  }

  return {
    token,
    user: {
      id: payload.sub,
      role: payload.role,
      username: payload.name,
    },
  };
}

function decodeJwtPayload(token: string): JwtPayload | null {
  const tokenParts = token.split('.');

  if (tokenParts.length !== 3) {
    return null;
  }

  try {
    const payloadJson = decodeBase64Url(tokenParts[1]);
    const payload = JSON.parse(payloadJson) as unknown;

    if (typeof payload !== 'object' || payload === null || Array.isArray(payload)) {
      return null;
    }

    return payload as JwtPayload;
  } catch {
    return null;
  }
}

function decodeBase64Url(value: string): string {
  const base64 = value.replace(/-/g, '+').replace(/_/g, '/');
  const paddedBase64 = base64.padEnd(Math.ceil(base64.length / 4) * 4, '=');
  const binaryValue = atob(paddedBase64);
  const bytes = Uint8Array.from(binaryValue, (character) => character.charCodeAt(0));

  return new TextDecoder().decode(bytes);
}

function isUserRole(value: unknown): value is UserRole {
  return value === 'Player' || value === 'Admin';
}

function isExpired(expirationTimestamp: number): boolean {
  return expirationTimestamp * 1000 <= Date.now();
}
