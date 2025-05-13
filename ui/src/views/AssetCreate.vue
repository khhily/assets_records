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

const batchId = ref('');
const assets = ref<Asset[]>([]);
const assetFormRef = ref<FormInstance>();
const isEditMode = ref(false);

// 检查是否是编辑模式
onMounted(() => {
  const routeBatchId = route.params.batchId as string;
  if (routeBatchId) {
    isEditMode.value = true;
    batchId.value = routeBatchId;

    // 查找该批次的资产
    const batch = assetStore.assetsByBatch.find((b) => b.id === routeBatchId);
    if (batch) {
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

const rules = reactive<FormRules>({
  batchId: [{ required: true, message: '请输入批次号', trigger: 'blur' }],
});

const assetRules = reactive<FormRules>({
  name: [{ required: true, message: '请输入资产名称', trigger: 'blur' }],
  type: [{ required: true, message: '请选择资产类型', trigger: 'change' }],
  amount: [
    { required: true, message: '请输入金额', trigger: 'blur' },
    { type: 'number', min: 0.01, message: '金额必须大于0', trigger: 'blur' },
  ],
  expiryDate: [
    {
      required: true,
      message: '请选择到期日期',
      trigger: 'change',
      validator: (rule, value, callback) => {
        const index = Number(rule.field.split('.')[1]);
        const asset = assets.value[index];
        const requiresExpiryDate = [
          AssetType.BankFixed,
          AssetType.DepositInvestment,
          AssetType.InsuranceInvestment,
        ].includes(asset.type);

        if (requiresExpiryDate && !asset.expiryDate) {
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
    type: AssetType.BankCurrent,
    amount: 0,
    expiryDate: '',
    batchId: batchId.value,
  });
};

const removeAsset = (index: number) => {
  assets.value.splice(index, 1);
};

const submitForm = async () => {
  if (!assetFormRef.value) return;

  try {
    await assetFormRef.value.validate();

    // 所有资产设置批次号
    for (const asset of assets.value) {
      asset.batchId = batchId.value;
    }

    if (isEditMode.value) {
      // 更新资产
      assetStore.updateAssets(batchId.value, assets.value);
      ElMessage.success('更新成功');
    } else {
      // 添加新资产
      assetStore.addAssets(assets.value);
      ElMessage.success('添加成功');
    }

    router.push('/');
  } catch (error) {
    // 验证失败
    console.error('表单验证失败', error);
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

    <el-form ref="assetFormRef" :model="{ batchId, assets }" :rules="rules" label-width="120px">
      <el-form-item label="批次号" prop="batchId" required>
        <el-input v-model="batchId" placeholder="请输入批次号" :disabled="isEditMode" />
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

          <el-form-item :label="'资产类型'" :prop="`assets.${index}.type`" :rules="assetRules.type" required>
            <el-select v-model="asset.type" placeholder="请选择资产类型" style="width: 100%">
              <el-option
                v-for="option in assetTypeOptions"
                :key="option.value"
                :label="option.label"
                :value="option.value"
              />
            </el-select>
          </el-form-item>

          <el-form-item :label="'金额'" :prop="`assets.${index}.amount`" :rules="assetRules.amount" required>
            <el-input-number v-model="asset.amount" :min="0.01" style="width: 100%" />
          </el-form-item>

          <el-form-item
            :label="'到期时间'"
            :prop="`assets.${index}.expiryDate`"
            :rules="assetRules.expiryDate"
            v-if="
              [AssetType.BankFixed, AssetType.DepositInvestment, AssetType.InsuranceInvestment].includes(asset.type)
            "
            required
          >
            <el-date-picker
              v-model="asset.expiryDate"
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
        <el-button type="primary" @click="submitForm">{{ isEditMode ? '保存' : '提交' }}</el-button>
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
