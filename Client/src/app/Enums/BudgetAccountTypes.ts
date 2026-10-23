export enum BudgetAccountTypes {
  PersonalPriority = 1,
  HighPriority = 2,
  LowPriority = 3,
  AveragePriority = 4,
  OutSideBudget = 5,
}

export const BudgetAccountTypeOptions = [
  { value: BudgetAccountTypes.PersonalPriority, label: 'Personal Priority' },
  { value: BudgetAccountTypes.HighPriority, label: 'High Priority' },
  { value: BudgetAccountTypes.LowPriority, label: 'Low Priority' },
  { value: BudgetAccountTypes.AveragePriority, label: 'Average Priority' },
  { value: BudgetAccountTypes.OutSideBudget, label: 'Out Side Budget' },
];
