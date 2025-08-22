import { Component, Input } from '@angular/core';
import { EvaluationListItem } from '../../../Interfaces/Response/EvaluationListItem';
import { EvaluationListItemComponent } from './evaluation-list-item/evaluation-list-item.component';

@Component({
  selector: 'app-evaluation-list',
  imports: [EvaluationListItemComponent],
  templateUrl: './evaluation-list.component.html',
  styleUrl: './evaluation-list.component.css',
})
export class EvaluationListComponent {
  @Input() evaluations: EvaluationListItem[] = [];
}
