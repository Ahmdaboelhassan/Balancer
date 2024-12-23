export interface Journal {
  id: number;
  code: number;
  notes: any;
  createdAt: string;
  lastUpdatedAt: any;
  type: number;
  accounts: Account[];
  costCenters: CostCenter[];
  detail: any;
  description: any;
  amount: number;
  periodId: number;
  costCenterId: any;
  creditAccountId: number;
  debitAccountId: number;
}

export interface Account {
  id: number;
  name: string;
}

export interface CostCenter {
  id: number;
  name: string;
}
