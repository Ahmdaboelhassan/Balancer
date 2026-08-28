export interface AccountsBalance {
  accountName: string;
  balance: string;
  accountId: number;
  isRevExp: boolean;
  isCredit: boolean;
  accountDescreption?: string;
  lastJournal?: LastAccountJournal;
}

export interface LastAccountJournal {
  journalId: number;
  type: number;
  name?: string;
  amount?: number;
  debitAccount?: string;
  creditAccount?: string;
  journalDate: string;
}
