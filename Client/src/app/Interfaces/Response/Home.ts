import { AccountsBalance } from './AccountsBalance';

export interface Home {
  accountsSummary: AccountsBalance[];
  lastFourPeriods: number[];
  currentAndLastMonthExpenses: number[];
  currentYearExpenses: number[];
  currentYearRevenues: number[];
}
