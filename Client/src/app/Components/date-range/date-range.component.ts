import { NgClass } from '@angular/common';
import {
  Component,
  EventEmitter,
  inject,
  input,
  Input,
  OnInit,
  Output,
} from '@angular/core';
import { FormsModule } from '@angular/forms';
import { JournalSearchModalComponent } from './journal-search-modal/journal-search-modal.component';
import { MatDialog } from '@angular/material/dialog';
import { JournalService } from '../../Services/journal.service';

@Component({
  selector: 'app-date-range',
  imports: [FormsModule, NgClass],
  templateUrl: './date-range.component.html',
  styleUrl: './date-range.component.css',
})
export class DateRangeComponent implements OnInit {
  @Output() dates = new EventEmitter<object>();
  @Input() showMaxLevelFilter = false;
  @Input() isYearRange = false;
  @Input() hideFromDate = false;
  @Input() journalFilter = false;
  @Input() maxLevel = '';
  readonly dialog = inject(MatDialog);
  readonly journalService = inject(JournalService);

  from: any;
  to: any;

  ngOnInit(): void {
    this.GetDefaultDate();
    this.journalService.advancedSearch$.subscribe({
      next: (result: any) => {
        this.from = result.from;
        this.to = result.to;
      },
    });
  }

  EmitDatesValue() {
    this.dates.emit({
      from: this.from,
      to: this.to,
      maxLevel: this.maxLevel,
    });
  }
  GetDefaultDate() {
    const currentDate = new Date();
    let startMonth = this.isYearRange ? 0 : currentDate.getMonth();
    let endMonth = this.isYearRange ? 12 : currentDate.getMonth() + 1;

    let firstDay = new Date(currentDate.getFullYear(), startMonth, 2);

    let lastDay = new Date(currentDate.getFullYear(), endMonth, 1);

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to, maxLevel: this.maxLevel });
  }

  DecrementMonth() {
    let f = this.from ? new Date(this.from) : new Date();
    f.setMonth(f.getMonth() - 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to, maxLevel: this.maxLevel });
  }

  IncrementMonth() {
    let f = this.from ? new Date(this.from) : new Date();

    f.setMonth(f.getMonth() + 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to, maxLevel: this.maxLevel });
  }

  GetCurrentDay() {
    const currentDate = new Date();
    this.from = this.formatLocalDate(currentDate);
    this.to = this.formatLocalDate(currentDate);
    this.dates.emit({ from: this.from, to: this.to, maxLevel: this.maxLevel });
  }

  GetDefaultWeekDates() {
    const currentDate = new Date();
    const day = currentDate.getDay();

    const diffToSaturday = day === 6 ? 0 : day + 1;

    const firstDayOfWeek = new Date(currentDate);
    firstDayOfWeek.setDate(currentDate.getDate() - diffToSaturday);

    const lastDayOfWeek = new Date(firstDayOfWeek);
    lastDayOfWeek.setDate(firstDayOfWeek.getDate() + 6);

    this.from = this.formatLocalDate(firstDayOfWeek);
    this.to = this.formatLocalDate(lastDayOfWeek);
    this.dates.emit({ from: this.from, to: this.to, maxLevel: this.maxLevel });
  }
  private formatLocalDate(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  openDialog() {
    this.dialog.open(JournalSearchModalComponent, {
      maxHeight: '90vh',
      autoFocus: false,
      data: {
        from: this.from,
        to: this.to,
      },
    });
  }
}
