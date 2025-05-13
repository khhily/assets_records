<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { useAssetStore } from '../stores/assetStore';
import { Asset, AssetType } from '../types';
import { ElMessage } from 'element-plus';
import type { FormInstance, FormRules } from 'element-plus';

const router = useRouter();
const route = useRoute();
const assetStore = useAssetStore();

const batchId = ref<number | null>(null);
const batchNo = ref('');
const assets = ref<Asset[]>([]);
const assetFormRef = ref<FormInstance>();
const isEditMode = ref(false);
const loading = ref(false);

// 检查是否是编辑模式
onMounted(async () => {
  // 先加载所有资产数据
  if (assetStore.assets.length === 0) {
    await assetStore.fetchAssets();
  }

  const routeBatchId = route.params.batchId ? Number(route.params.batchId) : null;
  if (routeBatchId) {
    isEditMode.value = true;
    batchId.value = routeBatchId;

    // 查找该批次的资产
    const batch = assetStore.assetsByBatch.find((b) => b.id === routeBatchId);
    if (batch) {
      batchNo.value = batch.batchNo;
      // 复制资产数据以避免直接修改 store 中的数据
      assets.value = batch.assets.map((asset) => ({ ...asset }));
    } else {
      ElMessage.error('未找到该批次');
      router.push('/');
    }
  } else {
    // 新建模式，添加一个空资产表单
    addAssetForm();
  }
});

// 移除批次号验证规则，只在编辑模式下需要
const rules = reactive<FormRules>({});

const assetRules = reactive<FormRules>({
  name: [{ required: true, message: '请输入资产名称', trigger: 'blur' }],
  assetType: [{ required: true, message: '请选择资产类型', trigger: 'change' }],
  amount: [
    { required: true, message: '请输入金额', trigger: 'blur' },
    { type: 'integer', min: 1, message: '金额必须是大于0的整数', trigger: 'blur' },
  ],
  maturityDate: [
    {
      required: true,
      message: '请选择到期日期',
      trigger: 'change',
      validator: (rule, value, callback) => {
        const index = Number(rule.field.split('.')[1]);
        const asset = assets.value[index];
        const requiresMaturityDate = [
          AssetType.BankFixed,
          AssetType.DepositInvestment,
          AssetType.InsuranceInvestment,
        ].includes(asset.assetType);

        if (requiresMaturityDate && !asset.maturityDate) {
          callback(new Error('请选择到期日期'));
        } else {
          callback();
        }
      },
    },
  ],
});

const assetTypeOptions = [
  { value: AssetType.BankCurrent, label: '银行活期' },
  { value: AssetType.BankFixed, label: '银行定期' },
  { value: AssetType.DepositInvestment, label: '存款型理财' },
  { value: AssetType.InsuranceInvestment, label: '保险型理财' },
  { value: AssetType.Cash, label: '现金' },
  { value: AssetType.ForeignDebt, label: '外债' },
];

const addAssetForm = () => {
  assets.value.push({
    name: '',
    assetType: AssetType.BankCurrent,
    amount: 0,
    maturityDate: null,
    batchId: batchId.value || 0, // 在新建模式下，这个值会是0
  });
};

const removeAsset = (index: number) => {
  assets.value.splice(index, 1);
};

const submitForm = async () => {
  if (!assetFormRef.value) return;

  try {
    await assetFormRef.value.validate();
    loading.value = true;

    if (isEditMode.value && batchId.value) {
      // 编辑模式：设置批次号并更新资产
      for (const asset of assets.value) {
        asset.batchId = batchId.value;
      }
      await assetStore.updateAssets(batchId.value, assets.value);
      ElMessage.success('更新成功');
    } else {
      // 新建模式：不设置批次号，由后端管理
      await assetStore.addAssets(assets.value);
      ElMessage.success('添加成功');
    }

    router.push('/');
  } catch (error) {
    // 验证失败或API错误
    console.error('操作失败', error);
    ElMessage.error('操作失败，请检查表单或网络连接');
  } finally {
    loading.value = false;
  }
};

const goBack = () => {
  router.push('/');
};
</script>

<template>
  <div class="asset-create-container">
    <div class="header">
      <h1>{{ isEditMode ? '编辑资产' : '添加资产' }}</h1>
      <el-button @click="goBack">返回</el-button>
    </div>

    <el-form
      ref="assetFormRef"
      :model="{ batchId, batchNo, assets }"
      :rules="rules"
      label-width="120px"
      @submit.native.prevent="submitForm"
    >
      <!-- 只在编辑模式下显示批次号 -->
      <el-form-item v-if="isEditMode" label="批次号" prop="batchNo">
        <el-input v-model="batchNo" disabled />
      </el-form-item>

      <div class="assets-section">
        <h2>资产列表</h2>

        <div v-for="(asset, index) in assets" :key="index" class="asset-form">
          <div class="asset-form-header">
            <h3>资产 #{{ index + 1 }}</h3>
            <el-button type="danger" size="small" @click="removeAsset(index)" :disabled="assets.length === 1">
              删除
            </el-button>
          </div>

          <el-form-item :label="'资产名称'" :prop="`assets.${index}.name`" :rules="assetRules.name" required>
            <el-input v-model="asset.name" placeholder="请输入资产名称" />
          </el-form-item>

          <el-form-item :label="'资产类型'" :prop="`assets.${index}.assetType`" :rules="assetRules.assetType" required>
            <el-select v-model="asset.assetType" placeholder="请选择资产类型" style="width: 100%">
              <el-option
                v-for="option in assetTypeOptions"
                :key="option.value"
                :label="option.label"
                :value="option.value"
              />
            </el-select>
          </el-form-item>

          <el-form-item :label="'金额'" :prop="`assets.${index}.amount`" :rules="assetRules.amount" required>
            <el-input-number
              v-model="asset.amount"
              :min="1"
              :precision="0"
              :step="1"
              controls-position="right"
              style="width: 100%"
            />
          </el-form-item>

          <el-form-item
            :label="'到期时间'"
            :prop="`assets.${index}.maturityDate`"
            :rules="assetRules.maturityDate"
            v-if="
              [AssetType.BankFixed, AssetType.DepositInvestment, AssetType.InsuranceInvestment].includes(
                asset.assetType
              )
            "
            required
          >
            <el-date-picker
              v-model="asset.maturityDate"
              type="date"
              placeholder="选择到期日期"
              style="width: 100%"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
        </div>

        <!-- 添加资产按钮移到这里 -->
        <div class="add-asset-button">
          <el-button type="primary" @click="addAssetForm" icon="el-icon-plus" size="large">添加资产</el-button>
        </div>
      </div>

      <el-form-item class="form-actions">
        <el-button type="primary" native-type="submit" :loading="loading">{{ isEditMode ? '保存' : '提交' }}</el-button>
        <el-button @click="goBack">取消</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<style scoped>
.asset-create-container {
  padding: 20px;
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.assets-section {
  margin-top: 20px;
  border: 1px solid #ebeef5;
  border-radius: 4px;
  padding: 20px;
}

.asset-form {
  border: 1px dashed #ccc;
  border-radius: 4px;
  padding: 20px;
  margin-bottom: 20px;
}

.asset-form-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.add-asset-button {
  display: flex;
  justify-content: center;
  margin: 20px 0;
}

.form-actions {
  margin-top: 20px;
  display: flex;
  justify-content: center;
}

/* 移动端适配 */
@media (max-width: 768px) {
  .asset-create-container {
    padding: 10px;
  }

  .asset-form {
    padding: 15px;
  }

  .el-form-item {
    margin-bottom: 15px;
  }
}
</style>
