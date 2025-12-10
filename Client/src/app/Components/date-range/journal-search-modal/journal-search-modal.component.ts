import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialogClose,
  MatDialogRef,
} from '@angular/material/dialog';
import { JournalService } from '../../../Services/journal.service';
import { Inject } from '@angular/core';

@Component({
  selector: 'app-journal-search-modal',
  imports: [MatDialogClose, ReactiveFormsModule],
  templateUrl: './journal-search-modal.component.html',
  styleUrl: './journal-search-modal.component.css',
})
export class JournalSearchModalComponent {
  journalForm!: FormGroup;
  from: string;
  to: string;
  constructor(
    private fb: FormBuilder,
    private journalService: JournalService,
    private dialogRef: MatDialogRef<JournalSearchModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.intializeForm(data.from, data.to);
  }

  Search() {
    this.journalService.advancedSearch$.next(this.journalForm.value);
    this.dialogRef.close();
  }

  intializeForm(from, to) {
    this.journalForm = this.fb.group({
      key: [{ value: '', disabled: true }],
      filterByDate: [true],
      filterByKey: [false],
      from: from,
      to: to,
      orderBy: ['2'],
      type: ['0'],
    });
  }

  ChangeFilterByDate() {
    if (this.journalForm.get('filterByDate').value) {
      this.journalForm.get('from')?.enable();
      this.journalForm.get('to')?.enable();
    } else {
      this.journalForm.get('from')?.disable();
      this.journalForm.get('to')?.disable();
    }
  }

  ChangeFilterByKey() {
    if (this.journalForm.get('filterByKey').value) {
      this.journalForm.get('key')?.enable();
      this.journalForm.get('from')?.disable();
      this.journalForm.get('to')?.disable();
      this.journalForm.patchValue({ filterByDate: false });
    } else {
      this.journalForm.get('key')?.disable();
    }
  }
  GetJournalsSummery() {
    this.journalService.journalsSummary$.next(true);
    this.dialogRef.close();
  }
}
