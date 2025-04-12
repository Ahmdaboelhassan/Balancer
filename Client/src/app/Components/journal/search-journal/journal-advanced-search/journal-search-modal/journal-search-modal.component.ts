import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatDialogClose, MatDialogRef } from '@angular/material/dialog';
import { JournalService } from '../../../../../Services/journal.service';

@Component({
  selector: 'app-journal-search-modal',
  imports: [MatDialogClose, ReactiveFormsModule],
  templateUrl: './journal-search-modal.component.html',
  styleUrl: './journal-search-modal.component.css',
})
export class JournalSearchModalComponent implements OnInit {
  journalForm!: FormGroup;

  constructor(
    private fb: FormBuilder,
    private journalService: JournalService,
    private dialogRef: MatDialogRef<JournalSearchModalComponent>
  ) {}

  ngOnInit(): void {
    this.intializeForm();
  }

  Search() {
    this.journalService.advancedSearch$.next(this.journalForm.value);
    this.dialogRef.close();
  }

  intializeForm() {
    const currentDate = new Date();
    const firstDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth(),
      2
    );

    const lastDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() + 1,
      1
    );

    this.journalForm = this.fb.group({
      key: [{ value: '', disabled: true }],
      filterByDate: [true],
      filterByKey: [false],
      from: firstDay.toISOString().split('T')[0],
      to: lastDay.toISOString().split('T')[0],
      orderBy: ['1'],
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
    } else {
      this.journalForm.get('key')?.disable();
    }
  }
}
