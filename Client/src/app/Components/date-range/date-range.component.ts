import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-date-range',
  imports: [FormsModule],
  templateUrl: './date-range.component.html',
  styleUrl: './date-range.component.css',
})
export class DateRangeComponent implements OnInit {
  @Output() dates = new EventEmitter<object>();
  @Input() showMaxLevelFilter = false;
  @Input() isYearRange = false;

  from: any;
  to: any;
  maxLevel: any = '';

  ngOnInit(): void {
    this.GetDefaultDate();
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
    let f = new Date(this.from);
    f.setMonth(f.getMonth() - 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to, maxLevel: this.maxLevel });
  }
  IncrementMonth() {
    let f = new Date(this.from);
    f.setMonth(f.getMonth() + 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to, maxLevel: this.maxLevel });
  }
}
