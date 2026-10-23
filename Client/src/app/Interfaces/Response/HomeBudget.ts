import { BudgetProgress } from './BudgetProgress';

export interface HomeBudget {
  budgetProgress: BudgetProgress[];
  currentCash?: BudgetProgress | null;
  otherExpenses: number;
  overBudget: number;
  availableFunds: number;
  otherExpensesTarget: number;
  totalRemains: number;
  currentCashBudgetAfterRemian: number;
}
