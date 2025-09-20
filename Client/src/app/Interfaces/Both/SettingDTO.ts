export interface SettingDTO {
    id: number;
    defaultCreditAccount: number | null;
    defaultDebitAccount: number | null;
    defaultPeriodDays?: number | null; // Optional with default value of 7
    defaultDayRate: number;
    expensesAccount: number | null;
    revenueAccount: number | null;
    assetsAccount: number | null;
    currentAssetsAccount: number | null;
    fixedAssetsAccount: number | null;
    currentCashAccount: number | null;
    liabilitiesAccount: number | null;
    banksAccount: number | null;
    drawersAccount: number | null;
    levelOneDigits: number;
    levelTwoDigits: number;
    levelThreeDigits: number;
    levelFourDigits: number;
    levelFiveDigits: number;
    notBudgetCostCenter: number;
    maxAccountLevel: number;
}