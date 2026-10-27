import axios from 'axios';
import { getAccessTokenAsync } from '../auth/authTokenStorage';

export const apiBaseUrl = process.env.EXPO_PUBLIC_API_URL;
const requestTimeoutMilliseconds = 10000;

export const apiClient = axios.create({
  baseURL: apiBaseUrl,
  timeout: requestTimeoutMilliseconds,
});

apiClient.interceptors.request.use(async (config) => {
  const token = await getAccessTokenAsync();

  if (token !== null) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});
