export interface CreateJournal {
  id: number;
  detail: string;
  description: string;
  amount: number;
  periodId: number;
  costCenterId: number;
  secondCostCenterId: number;
  creditAccountId: number;
  debitAccountId: number;
  createdAt: string;
}
