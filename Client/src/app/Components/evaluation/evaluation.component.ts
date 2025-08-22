import { Component } from '@angular/core';
import { EvaluationListItem } from '../../Interfaces/Response/EvaluationListItem';
import { EvaluationService } from '../../Services/evaluation.service';
import { Title } from '@angular/platform-browser';
import { DateRangeComponent } from '../date-range/date-range.component';
import { EvaluationListComponent } from './evaluation-list/evaluation-list.component';

@Component({
  selector: 'app-evaluation',
  imports: [DateRangeComponent, EvaluationListComponent],
  templateUrl: './evaluation.component.html',
  styleUrl: './evaluation.component.css',
})
export class EvaluationComponent {
  items: EvaluationListItem[] = [];

  constructor(
    private evaluationService: EvaluationService,
    private titleService: Title
  ) {
    this.titleService.setTitle('Periods');
  }

  GetEvaluationsByDate(date: object | any) {
    this.evaluationService.GetAll(date.from, date.to).subscribe({
      next: (result: EvaluationListItem[] | any) => {
        this.items = result;
      },
    });
  }
}
