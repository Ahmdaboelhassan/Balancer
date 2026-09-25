export interface CreatePeriod {
  id: number;
  from: string;
  to: string;
  daysCount: number;
  isCurrent: boolean;
  periodBudget?: number;
  notes: string;
}
