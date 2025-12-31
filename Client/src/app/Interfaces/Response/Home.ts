import { AccountsBalance } from './AccountsBalance';
import { BudgetProgress } from './BudgetProgress';

export interface Home {
  accountsSummary: AccountsBalance[];
  lastPeriods: number[];
  journalsTypesSummary: number[];
  expenses: number[];
  revenues: number[];
  monthsNames: string[];
  budgetProgress: BudgetProgress[];
  dayRate: number;
  periodDays: number;
  availableFunds: number;
  otherExpensesTarget: number;
}
