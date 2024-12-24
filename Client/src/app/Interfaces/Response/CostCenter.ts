import { CreateCostCenter } from '../Request/CreateCostCenter';

export interface CostCenter extends CreateCostCenter {
  createdAt: string;
}
