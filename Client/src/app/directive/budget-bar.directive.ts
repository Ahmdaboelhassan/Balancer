import {
  Directive,
  ElementRef,
  Input,
  OnChanges,
  SimpleChanges,
} from '@angular/core';

@Directive({
  selector: '[BudgetBalue]',
  standalone: true,
})
export class BudgetBarDirective implements OnChanges {
  @Input() amount: number;
  @Input() target: number;

  constructor(private el: ElementRef) {
    this.el.nativeElement.style.transition = 'width 1.5s ease-in-out';
  }

  ngOnChanges(changes: SimpleChanges) {
    if (
      (changes['amount'] || changes['target']) &&
      this.amount &&
      this.target
    ) {
      let percentage = (this.amount / this.target) * 100;

      if (percentage > 100) percentage = 100;
      if (percentage < 0) percentage = 2;

      this.el.nativeElement.style.width = `${percentage}%`;
    }
  }
}
