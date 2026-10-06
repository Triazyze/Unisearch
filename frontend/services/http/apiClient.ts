import axios from 'axios';

export const apiBaseUrl = process.env.EXPO_PUBLIC_API_URL;

export const apiClient = axios.create({
  baseURL: apiBaseUrl,
});
