export interface CreatePeriod {
  id: number;
  from: string;
  to: string;
  daysCount: number;
  periodBudget?: number;
  notes: string;
}
