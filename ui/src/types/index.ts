// 资产类型枚举
export enum AssetType {
  BankCurrent = 1,
  BankFixed = 2,
  DepositInvestment = 3,
  InsuranceInvestment = 4,
  Cash = 5,
  ForeignDebt = 6,
}

// 资产类型名称映射
export const AssetTypeNames: { [key: number]: string } = {
  [AssetType.BankCurrent]: '银行活期',
  [AssetType.BankFixed]: '银行定期',
  [AssetType.DepositInvestment]: '存款型理财',
  [AssetType.InsuranceInvestment]: '保险型理财',
  [AssetType.Cash]: '现金',
  [AssetType.ForeignDebt]: '外债',
};

// 资产接口
export interface Asset {
  id?: number;
  name: string;
  type: AssetType;
  amount: number;
  expiryDate: string;
  createdAt?: string;
  updatedAt?: string;
  batchId: string;
}

// 批次接口
export interface Batch {
  id: string;
  assets: Asset[];
}
