export interface AccountStatementDetail {
  detail: string;
  notes: string;
  debit: number;
  credit: number;
  balance: number;
  costcenter: string;
  journalId: number;
  periodId: number;
  date: string;
}
