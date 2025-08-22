import { Component, Input } from '@angular/core';
import { EvaluationListItem } from '../../../../Interfaces/Response/EvaluationListItem';
import { RouterLink } from '@angular/router';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-evaluation-list-item',
  imports: [RouterLink, NgClass],
  templateUrl: './evaluation-list-item.component.html',
  styleUrl: './evaluation-list-item.component.css',
})
export class EvaluationListItemComponent {
  @Input() item: EvaluationListItem | null = null;
}
