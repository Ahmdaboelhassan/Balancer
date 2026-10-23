import { BudgetAccountTypes } from '../../Enums/BudgetAccountTypes';

export interface BudgetSummaryDTO {
  id: number;
  name: string;
  budget?: number | null;
  spent: number;
  remains?: number | null;
  spentPercentage?: number | null;
  remainPercentage?: number | null;
  isOtherExpenses: boolean;
  color?: string | null;
  type?: BudgetAccountTypes | null;
  bestBudget?: number | null;
}

export interface BudgetSummaryReportDTO {
  periods: BudgetSummaryDTO[];
  payOnceAccounts: BudgetSummaryDTO[];
  incrementalAccounts: BudgetSummaryDTO[];
  outSideBudgetAccounts: BudgetSummaryDTO[];
  savings: BudgetSummaryDTO[];
}
