import { AccountsBalance } from './AccountsBalance';

export interface Home {
  accountsSummary: AccountsBalance[];
  currentAndLastMonthExpenses: number[];
  currentYearExpenses: number[];
  currentYearRevenues: number[];
}
