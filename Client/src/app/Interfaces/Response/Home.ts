import { AccountsBalance } from './AccountsBalance';
import { HomeBudget } from './HomeBudget';

export interface Home {
  accountsSummary: AccountsBalance[];
  lastPeriods: number[];
  journalsTypesSummary: number[];
  expenses: number[];
  revenues: number[];
  monthsNames: string[];
  budget: HomeBudget;
  dayRate: number;
  periodDays: number;
  outSideBudgetCostCenterId?: number | null;
}
