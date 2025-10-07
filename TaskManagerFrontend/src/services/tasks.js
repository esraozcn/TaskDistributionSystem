import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5000/api';

// Axios instance with auth token
const api = axios.create({
  baseURL: API_BASE,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor to add auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export async function getWeeklyTasks(companyId, date = null) {
  try {
    const params = date ? `?date=${date}` : '';
    const response = await api.get(`/Tasks/weekly/${companyId}${params}`);
    console.log('API Response:', response.data); // Debug
    return response.data;
  } catch (error) {
    console.error('API Error:', error.response?.data || error.message);
    throw new Error(error.response?.data?.message || 'Görevler getirilirken hata oluştu');
  }
}

export async function generateWeeklyTasks(companyId, date = null) {
  try {
    const params = date ? `?date=${date}` : '';
    const response = await api.post(`/Tasks/generate/${companyId}${params}`);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Görev oluşturulurken hata oluştu');
  }
}

export async function updateTaskStatus(taskId, status) {
  try {
    const response = await api.put(`/Tasks/${taskId}/status`, { status });
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Durum güncellenirken hata oluştu');
  }
}

export async function getTaskStatistics(companyId, date = null) {
  try {
    const params = date ? `?date=${date}` : '';
    const response = await api.get(`/Tasks/statistics/${companyId}${params}`);
    return response.data;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'İstatistikler getirilirken hata oluştu');
  }
}
