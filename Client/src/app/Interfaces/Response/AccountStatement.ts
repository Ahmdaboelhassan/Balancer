import { AccountStatementDetail } from './AccountStatementDetail';

export interface AccountStatement {
  details: AccountStatementDetail[];
  accountType: string;
  amount: string;
  accountName: string;
  from: string;
  to: string;
}
