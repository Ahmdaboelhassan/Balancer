import { BudgetAccountTypes } from '../../Enums/BudgetAccountTypes';
import { PaidMethods } from '../../Enums/PaidMethods';

export interface BudgetAccountSettingsDTO {
  id: number;
  accountId: number;
  budget: number;
  bestBudget: number;
  maximumBudget: number;
  paidMethodId: PaidMethods;
  typeId: BudgetAccountTypes;
  color: string;
  displayName: string;
  notes?: string;
}
