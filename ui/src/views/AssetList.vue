<script setup lang="ts">
import { useAssetStore } from '../stores/assetStore';
import { AssetTypeNames } from '../types';
import { useRouter } from 'vue-router';
import { Plus, DataAnalysis } from '@element-plus/icons-vue';

const assetStore = useAssetStore();
const router = useRouter();

const navigateToCreate = () => {
  router.push('/create');
};

const navigateToChart = () => {
  router.push('/chart');
};

const navigateToEdit = (batchId: string) => {
  router.push(`/edit/${batchId}`);
};
</script>

<template>
  <div class="asset-list-container">
    <div class="header">
      <h1>资产记录列表</h1>
      <div class="header-buttons">
        <el-tooltip content="添加资产" placement="top">
          <el-button type="primary" circle @click="navigateToCreate">
            <el-icon><Plus /></el-icon>
          </el-button>
        </el-tooltip>
        <el-tooltip content="查看图表" placement="top">
          <el-button type="info" circle @click="navigateToChart">
            <el-icon><DataAnalysis /></el-icon>
          </el-button>
        </el-tooltip>
      </div>
    </div>

    <div v-if="assetStore.assetsByBatch.length === 0" class="empty-state">
      <el-empty description="暂无资产记录" />
    </div>

    <div v-else class="batch-list">
      <div v-for="batch in assetStore.assetsByBatch" :key="batch.id" class="batch-item">
        <div class="batch-header">
          <h2>批次: {{ batch.id }}</h2>
          <el-button type="primary" size="small" @click="navigateToEdit(batch.id)">编辑</el-button>
        </div>

        <el-table :data="batch.assets" border style="width: 100%">
          <el-table-column prop="name" label="资产名称" />
          <el-table-column label="资产类型">
            <template #default="scope">
              {{ AssetTypeNames[scope.row.type] }}
            </template>
          </el-table-column>
          <el-table-column prop="amount" label="金额" />
          <el-table-column prop="expiryDate" label="到期时间" />
          <el-table-column prop="createdAt" label="创建时间" />
          <el-table-column prop="updatedAt" label="最后修改时间" />
        </el-table>
      </div>
    </div>
  </div>
</template>

<style scoped>
.asset-list-container {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.header-buttons {
  display: flex;
  gap: 10px;
}

.batch-list {
  display: flex;
  flex-direction: column;
  gap: 30px;
}

.batch-item {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 20px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.batch-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 10px;
}

.empty-state {
  margin-top: 100px;
}

/* 移动端适配 */
@media (max-width: 768px) {
  .header {
    flex-direction: column;
    align-items: flex-start;
    gap: 10px;
  }

  .header h1 {
    margin-bottom: 10px;
  }
}
</style>
