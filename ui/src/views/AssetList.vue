<script setup lang="ts">
import { onMounted } from 'vue';
import { useAssetStore } from '../stores/assetStore';
import { AssetTypeNames } from '../types';
import { useRouter } from 'vue-router';
import { Plus, DataAnalysis, Delete } from '@element-plus/icons-vue';
import { ElMessageBox, ElMessage } from 'element-plus';

const assetStore = useAssetStore();
const router = useRouter();

// 组件挂载时加载数据
onMounted(async () => {
  await assetStore.fetchAssets();
});

const navigateToCreate = () => {
  router.push('/create');
};

const navigateToChart = () => {
  router.push('/chart');
};

const navigateToEdit = (batchId: number) => {
  router.push(`/edit/${batchId}`);
};

const confirmDelete = (batchId: number) => {
  ElMessageBox.confirm('确定要删除这个批次的所有资产吗？此操作不可恢复。', '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning',
  })
    .then(async () => {
      try {
        await assetStore.removeBatch(batchId);
        ElMessage.success('删除成功');
      } catch (error) {
        ElMessage.error('删除失败');
      }
    })
    .catch(() => {
      // 用户取消删除
    });
};

const formatDate = (dateString?: string): string => {
  if (!dateString) return '';

  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');

  return `${year}-${month}-${day}`;
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

    <div v-if="assetStore.loading" class="loading-state">
      <el-skeleton :rows="5" animated />
    </div>

    <div v-else-if="assetStore.error" class="error-state">
      <el-alert :title="assetStore.error" type="error" />
    </div>

    <div v-else-if="assetStore.assetsByBatch.length === 0" class="empty-state">
      <el-empty description="暂无资产记录" />
    </div>

    <div v-else class="batch-list">
      <div v-for="batch in assetStore.assetsByBatch" :key="batch.id" class="batch-item">
        <div class="batch-header">
          <div class="batch-info">
            <h2>批次: {{ batch.batchNo }}</h2>
            <div class="batch-meta">
              <span class="batch-amount"
                >总金额:
                <span class="amount-value">{{ batch.totalAmount?.toLocaleString() || '0' }}</span>
                元</span
              >
              <span class="batch-time">最后更新: {{ formatDate(batch.lastModifiedTime) }}</span>
            </div>
          </div>
          <div class="batch-actions">
            <el-button type="primary" size="small" @click="navigateToEdit(batch.id)"
              >编辑</el-button
            >
            <el-button type="danger" size="small" @click="confirmDelete(batch.id)">
              <el-icon><Delete /></el-icon>
            </el-button>
          </div>
        </div>

        <!-- 桌面端表格显示 -->
        <div class="desktop-view">
          <el-table :data="batch.assets" border style="width: 100%">
            <el-table-column prop="name" label="资产名称" />
            <el-table-column label="资产类型">
              <template #default="scope"> {{ AssetTypeNames[scope.row.assetType] }} </template>
            </el-table-column>
            <el-table-column label="金额">
              <template #default="scope"> {{ scope.row.amount.toLocaleString() }} </template>
            </el-table-column>
            <el-table-column label="到期时间">
              <template #default="scope"> {{ formatDate(scope.row.maturityDate) }} </template>
            </el-table-column>
          </el-table>
        </div>

        <!-- 移动端卡片显示 -->
        <div class="mobile-view">
          <div v-for="asset in batch.assets" :key="asset.id" class="asset-card">
            <div class="asset-card-item">
              <span class="asset-card-label">资产名称:</span>
              <span class="asset-card-value">{{ asset.name }}</span>
            </div>
            <div class="asset-card-item">
              <span class="asset-card-label">资产类型:</span>
              <span class="asset-card-value">{{ AssetTypeNames[asset.assetType] }}</span>
            </div>
            <div class="asset-card-item">
              <span class="asset-card-label">金额:</span>
              <span class="asset-card-value amount">{{ asset.amount.toLocaleString() }}</span>
            </div>
            <div v-if="asset.maturityDate" class="asset-card-item">
              <span class="asset-card-label">到期时间:</span>
              <span class="asset-card-value">{{ formatDate(asset.maturityDate) }}</span>
            </div>
          </div>
        </div>
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

.batch-actions {
  display: flex;
  gap: 10px;
}

.empty-state,
.loading-state,
.error-state {
  margin-top: 100px;
}

/* 桌面端和移动端视图切换 */
.mobile-view {
  display: none;
}

.desktop-view {
  display: block;
}

/* 移动端卡片样式 */
.asset-card {
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 12px;
  margin-bottom: 10px;
  background-color: #ffffff;
}

.asset-card-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
}

.asset-card-label {
  color: #909399;
  font-size: 14px;
}

.asset-card-value {
  font-weight: 500;
  color: #303133;
}

.asset-card-value.amount {
  color: #f56c6c;
  font-weight: bold;
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

  .batch-meta {
    flex-direction: column;
    gap: 5px;
  }

  .mobile-view {
    display: block;
  }

  .desktop-view {
    display: none;
  }

  .batch-item {
    padding: 15px;
  }
}

.batch-meta {
  display: flex;
  gap: 30px;
  color: #606266;
  font-size: 14px;
}

.batch-amount {
  font-weight: bold;
}

.amount-value {
  color: #f56c6c;
  font-weight: bold;
}

.batch-time {
  color: #909399;
}
</style>
