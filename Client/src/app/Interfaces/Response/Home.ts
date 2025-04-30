import { AccountsBalance } from './AccountsBalance';

export interface Home {
  accountsSummary: AccountsBalance[];
  lastPeriods: number[];
  currentAndLastMonthExpenses: number[];
  currentYearExpenses: number[];
  currentYearRevenues: number[];
  periodExpensesTarget: number;
  otherExpensesTarget: number;
  gamieaLiabilitiesTarget: number;
  monthlySavingsTarget: number;
  periodExpensesAmount: number;
  otherExpensesAmount: number;
  gamieaLiabilitiesAmount: number;
  monthlySavingsAmount: number;
  dayRate: number;
  periodDays: number;
  availableFunds: number;
}
