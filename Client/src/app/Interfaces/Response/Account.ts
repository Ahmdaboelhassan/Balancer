import { CreateAccount } from '../Request/CreateAccount';
import { AccountSelectList } from './AccountSelectList';

export interface Account extends CreateAccount {
  accountNumber: string;
  parentNumber: any;
  parentName: any;
  isParent: boolean;
  isArchive: boolean;
  accounts: AccountSelectList[];
}
