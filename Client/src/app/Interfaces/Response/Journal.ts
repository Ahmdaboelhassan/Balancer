import { AccountSelectList } from './AccountSelectList';
import { CostCenterSelectList } from './CostCenterSelectList';

export interface Journal {
  id: number;
  code: number;
  notes: any;
  createdAt: string;
  lastUpdatedAt: any;
  type: number;
  accounts: AccountSelectList[];
  costCenters: CostCenterSelectList[];
  detail: any;
  description: any;
  amount: number;
  periodId: number;
  costCenterId: any;
  creditAccountId: number;
  debitAccountId: number;
}
