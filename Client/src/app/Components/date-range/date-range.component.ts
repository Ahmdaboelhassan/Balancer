import {
  Component,
  EventEmitter,
  OnInit,
  Output,
  viewChild,
} from '@angular/core';

@Component({
  selector: 'app-date-range',
  imports: [],
  templateUrl: './date-range.component.html',
  styleUrl: './date-range.component.css',
})
export class DateRangeComponent implements OnInit {
  @Output() dates = new EventEmitter<object>();
  from: any;
  to: any;

  ngOnInit(): void {
    this.GetDefaultDate();
  }

  EmitDatesValue(from: HTMLInputElement, to: HTMLInputElement) {
    this.dates.emit({ from: from.value, to: to.value });
  }
  GetDefaultDate() {
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

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to });
  }
  DecrementMonth() {
    let f = new Date(this.from);
    f.setMonth(f.getMonth() - 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to });
  }
  IncrementMonth() {
    let f = new Date(this.from);
    f.setMonth(f.getMonth() + 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from = firstDay.toISOString().split('T')[0];
    this.to = lastDay.toISOString().split('T')[0];
    this.dates.emit({ from: this.from, to: this.to });
  }
}
