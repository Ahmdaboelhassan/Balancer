import { JournalListItem } from './JournalListItem';

export interface Periodjournals {
  id: number;
  name: string;
  from: string;
  to: string;
  journal: JournalListItem[];
}
