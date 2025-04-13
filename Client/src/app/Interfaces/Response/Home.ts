import { AccountsBalance } from './AccountsBalance';

export interface Home {
  accountsSummary: AccountsBalance[];
  lastPeriods: number[];
  currentAndLastMonthExpenses: number[];
  currentYearExpenses: number[];
  currentYearRevenues: number[];
}
