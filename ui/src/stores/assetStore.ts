import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { Asset, Batch, AssetBatchResponse } from '../types';
import {
  getAssets,
  createAssets as apiCreateAssets,
  updateAssets as apiUpdateAssets,
  deleteBatch,
} from '../api';

export const useAssetStore = defineStore('asset', () => {
  const assets = ref<AssetBatchResponse[]>([]);
  const loading = ref(false);
  const error = ref<string | null>(null);

  // 按批次分组的资产
  const assetsByBatch = computed(() => {
    return assets.value.map(item => ({
      id: item.batch.id,
      batchNo: item.batch.batchNo,
      createdTime: item.batch.createdTime,
      lastModifiedTime: item.batch.lastModifiedTime,
      totalAmount: item.batch.totalAmount,
      assets: item.assets,
    })) as Batch[];
  });

  // 从API加载资产
  async function fetchAssets() {
    loading.value = true;
    error.value = null;
    try {
      const data = await getAssets();
      assets.value = data;
    } catch (err) {
      error.value = '加载资产失败';
      console.error(err);
    } finally {
      loading.value = false;
    }
  }

  // 添加资产
  async function addAssets(newAssets: Asset[]) {
    loading.value = true;
    error.value = null;
    try {
      // 不再需要转换字段名
      await apiCreateAssets(newAssets);
      await fetchAssets(); // 重新加载数据
    } catch (err) {
      error.value = '添加资产失败';
      console.error(err);
    } finally {
      loading.value = false;
    }
  }

  // 更新资产
  async function updateAssets(batchId: number, updatedAssets: Asset[]) {
    loading.value = true;
    error.value = null;
    try {
      // 不再需要转换字段名
      await apiUpdateAssets(batchId, updatedAssets);
      await fetchAssets(); // 重新加载数据
    } catch (err) {
      error.value = '更新资产失败';
      console.error(err);
    } finally {
      loading.value = false;
    }
  }

  // 删除批次
  async function removeBatch(batchId: number) {
    loading.value = true;
    error.value = null;
    try {
      await deleteBatch(batchId);
      await fetchAssets(); // 重新加载数据
    } catch (err) {
      error.value = '删除批次失败';
      console.error(err);
    } finally {
      loading.value = false;
    }
  }

  return {
    assets,
    assetsByBatch,
    loading,
    error,
    fetchAssets,
    addAssets,
    updateAssets,
    removeBatch,
  };
});
