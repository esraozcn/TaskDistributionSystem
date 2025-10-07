import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

export function saveSession({ token, user }) {
  if (token) localStorage.setItem('token', token);
  if (user) localStorage.setItem('user', JSON.stringify(user));
}

export function clearSession() {
  localStorage.removeItem('token');
  localStorage.removeItem('user');
}

export function getToken() {
  return localStorage.getItem('token');
}

export function getUser() {
  const raw = localStorage.getItem('user');
  return raw ? JSON.parse(raw) : null;
}

export async function loginApi(credentials) {
  try {
    const response = await axios.post(`${API_BASE}/Auth/login`, credentials);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Giriş yapılırken hata oluştu');
  }
}

export async function registerApi(userData) {
  try {
    const response = await axios.post(`${API_BASE}/Auth/register`, userData);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Kayıt olurken hata oluştu');
  }
}

export async function getCompaniesApi() {
  try {
    const response = await axios.get(`${API_BASE}/Auth/companies`);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Şirketler getirilirken hata oluştu');
  }
}
