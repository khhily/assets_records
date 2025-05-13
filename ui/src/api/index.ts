import axios from 'axios';
import type { Asset } from '../types';

// 创建 axios 实例
const api = axios.create({
  baseURL: '/api',
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
});

// 获取所有资产
export const getAssets = async () => {
  try {
    const response = await api.get('/Assets');
    return response.data;
  } catch (error) {
    console.error('获取资产失败:', error);
    throw error;
  }
};

// 创建资产
export const createAssets = async (assets: Asset[]) => {
  try {
    const response = await api.post('/Assets', { assets });
    return response.data;
  } catch (error) {
    console.error('创建资产失败:', error);
    throw error;
  }
};

// 更新资产
export const updateAssets = async (batchId: number, assets: Asset[]) => {
  try {
    const response = await api.put(`/Assets/${batchId}`, { assets });
    return response.data;
  } catch (error) {
    console.error('更新资产失败:', error);
    throw error;
  }
};

// 删除资产批次
export const deleteBatch = async (batchId: number) => {
  try {
    const response = await api.delete(`/Assets/${batchId}`);
    return response.data;
  } catch (error) {
    console.error('删除资产批次失败:', error);
    throw error;
  }
};
