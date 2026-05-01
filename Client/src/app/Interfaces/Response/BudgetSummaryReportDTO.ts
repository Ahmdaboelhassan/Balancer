export interface BudgetSummaryDTO {
  id: number;
  name: string;
  budget?: number | null;
  spent: number;
  remains?: number | null;
  spentPercentage?: number | null;
  remainPercentage?: number | null;
  isOtherExpenses: boolean;
}

export interface BudgetSummaryReportDTO {
  periods: BudgetSummaryDTO[];
  accounts: BudgetSummaryDTO[];
  savings: BudgetSummaryDTO[];
}
