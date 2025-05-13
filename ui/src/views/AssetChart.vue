<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { useAssetStore } from '../stores/assetStore';
import { use } from 'echarts/core';
import { CanvasRenderer } from 'echarts/renderers';
import { LineChart } from 'echarts/charts';
import { GridComponent, TooltipComponent, TitleComponent, LegendComponent } from 'echarts/components';
import VChart from 'vue-echarts';

// 注册 ECharts 组件
use([
  CanvasRenderer,
  LineChart,
  GridComponent,
  TooltipComponent,
  TitleComponent,
  LegendComponent
]);

const router = useRouter();
const assetStore = useAssetStore();

// 计算每个批次的总资产
const batchTotals = computed(() => {
  return assetStore.assetsByBatch.map(batch => {
    const total = batch.assets.reduce((sum, asset) => sum + asset.amount, 0);
    return {
      batchId: batch.id,
      total
    };
  });
});

// 图表选项
const chartOption = computed(() => {
  return {
    title: {
      text: '批次资产总额',
      left: 'center'
    },
    tooltip: {
      trigger: 'axis',
      formatter: '{b}: {c} 元'
    },
    xAxis: {
      type: 'category',
      data: batchTotals.value.map(item => item.batchId)
    },
    yAxis: {
      type: 'value',
      name: '金额 (元)'
    },
    series: [
      {
        name: '总资产',
        type: 'line',
        data: batchTotals.value.map(item => item.total),
        smooth: true,
        markPoint: {
          data: [
            { type: 'max', name: '最大值' },
            { type: 'min', name: '最小值' }
          ]
        }
      }
    ]
  };
});

const goBack = () => {
  router.push('/');
};
</script>

<template>
  <div class="chart-container">
    <div class="header">
      <h1>资产统计图表</h1>
      <el-button @click="goBack">返回</el-button>
    </div>

    <div v-if="batchTotals.length === 0" class="empty-state">
      <el-empty description="暂无资产数据" />
    </div>
    
    <div v-else class="chart-wrapper">
      <v-chart class="chart" :option="chartOption" autoresize />
    </div>
  </div>
</template>

<style scoped>
.chart-container {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.chart-wrapper {
  margin-top: 20px;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 20px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.chart {
  height: 400px;
  width: 100%;
}

.empty-state {
  margin-top: 100px;
}
</style>