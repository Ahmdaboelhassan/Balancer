import { BudgetAccountSettingsDTO } from './BudgetAccountSettingsDTO';

export interface BudgetSettingsDTO {
  otherExpensesTarget: number;
  addOnExpensesTarget: number;
  totalRevenue: number;
  totalIncome: number;
  savingTargetAccountId?: number | null;
  savingTargetDisplayName?: string | null;
  applyOverBudgetToFunds: boolean;
  budgetAccounts: BudgetAccountSettingsDTO[];
}
