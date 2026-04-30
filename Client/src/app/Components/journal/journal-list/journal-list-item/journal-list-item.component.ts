import { Component, Input } from '@angular/core';
import { JournalListItem } from '../../../../Interfaces/Response/JournalListItem';
import { RouterLink } from '@angular/router';
import { NgClass } from '@angular/common';

type JournalStyle = {
  icon: string;
  color: string;
};

@Component({
  selector: 'app-journal-list-item',
  imports: [RouterLink, NgClass],
  templateUrl: './journal-list-item.component.html',
  styleUrl: './journal-list-item.component.css',
})
export class JournalListItemComponent {
  @Input() journal: JournalListItem | any;

  styleMap: Record<number, JournalStyle> = {
    1: { icon: 'fa-solid fa-square-plus', color: 'text-green-500' },
    2: { icon: 'fa-solid fa-square-minus', color: 'text-red-400' },
    3: { icon: 'fa-solid fa-thumbtack', color: 'text-blue-400' },
    4: {
      icon: 'fa-solid fa-reply fa-flip-horizontal',
      color: 'text-orange-400',
    },
    5: { icon: 'fa-solid fa-wallet', color: 'text-purple-400' },
  };

  get journalStyle(): JournalStyle {
    return (
      this.styleMap[this.journal.type] ?? {
        icon: 'fa-solid fa-circle-question',
        color: 'text-gray-400',
      }
    );
  }
}
