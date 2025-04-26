import { AccountsBalance } from './AccountsBalance';

export interface Home {
  accountsSummary: AccountsBalance[];
  lastPeriods: number[];
  currentAndLastMonthExpenses: number[];
  currentYearExpenses: number[];
  currentYearRevenues: number[];
  periodExpensesTarget: Number;
  otherExpensesTarget: Number;
  gamieaLiabilitiesTarget: Number;
  monthlySavingsTarget: Number;
  periodExpensesAmount: Number;
  otherExpensesAmount: Number;
  gamieaLiabilitiesAmount: Number;
  monthlySavingsAmount: Number;
  dayRate: Number;
  periodDays: Number;
  availableFunds: Number;
}
