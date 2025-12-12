import { AccountsBalance } from './AccountsBalance';
import { BudgetProgress } from './BudgetProgress';

export interface Home {
  accountsSummary: AccountsBalance[];
  lastPeriods: number[];
  journalsTypesSummary: number[];
  currentYearExpenses: number[];
  currentYearRevenues: number[];
  budgetProgress: BudgetProgress[];
  dayRate: number;
  periodDays: number;
  availableFunds: number;
  otherExpensesTarget: number;
}
