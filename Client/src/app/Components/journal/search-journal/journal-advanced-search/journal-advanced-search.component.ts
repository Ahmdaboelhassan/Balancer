import { NgClass } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { JournalSearchModalComponent } from './journal-search-modal/journal-search-modal.component';

@Component({
  selector: 'app-journal-advanced-search',
  imports: [NgClass],
  templateUrl: './journal-advanced-search.component.html',
  styleUrl: './journal-advanced-search.component.css',
})
export class JournalAdvancedSearchComponent {
  @Output() Search = new EventEmitter<string>();
  @Input() Amount;
  readonly dialog = inject(MatDialog);

  openDialog() {
    this.dialog.open(JournalSearchModalComponent, {
      maxHeight: '90vh',
      autoFocus: false,
    });
  }

  Seach(key: HTMLInputElement) {
    this.Search.emit(key.value);
  }
}
