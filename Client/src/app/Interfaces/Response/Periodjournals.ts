import { JournalListItem } from './JournalListItem';

export interface Periodjournals {
  id: number;
  name: string;
  from: string;
  to: string;
  totalAmount: number;
  periodBudget?: number;
  remains?: number;
  journals: JournalListItem[];
}
