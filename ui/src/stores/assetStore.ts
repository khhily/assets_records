import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import type { Asset, Batch } from '../types';

export const useAssetStore = defineStore('asset', () => {
  const assets = ref<Asset[]>([]);

  // 按批次分组的资产
  const assetsByBatch = computed(() => {
    const batches: Record<string, Asset[]> = {};

    assets.value.forEach((asset) => {
      if (!batches[asset.batchId]) {
        batches[asset.batchId] = [];
      }
      batches[asset.batchId].push(asset);
    });

    return Object.entries(batches).map(([id, assets]) => ({
      id,
      assets,
    })) as Batch[];
  });

  // 添加资产
  function addAssets(newAssets: Asset[]) {
    const now = new Date().toISOString();
    newAssets.forEach((asset) => {
      assets.value.push({
        ...asset,
        id: Date.now() + Math.floor(Math.random() * 1000),
        createdAt: now,
        updatedAt: now,
      });
    });
  }

  // 更新资产
  function updateAssets(batchId: string, updatedAssets: Asset[]) {
    const now = new Date().toISOString();

    // 删除该批次的所有旧资产
    assets.value = assets.value.filter((asset) => asset.batchId !== batchId);

    // 添加更新后的资产
    updatedAssets.forEach((asset) => {
      assets.value.push({
        ...asset,
        batchId,
        updatedAt: now,
        // 如果是新资产则生成ID，否则保留原ID
        id: asset.id || Date.now() + Math.floor(Math.random() * 1000),
      });
    });
  }

  return {
    assets,
    assetsByBatch,
    addAssets,
    updateAssets,
  };
});
