import { AccountStatementDetail } from './AccountStatementDetail';

export interface AccountStatement {
  details: AccountStatementDetail[];
  accountType: string;
  amount: string;
}
