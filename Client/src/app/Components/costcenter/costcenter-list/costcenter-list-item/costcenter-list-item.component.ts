import { Component, Input } from '@angular/core';
import { CostCenter } from '../../../../Interfaces/Response/CostCenter';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-costcenter-list-item',
  imports: [RouterLink],
  templateUrl: './costcenter-list-item.component.html',
  styleUrl: './costcenter-list-item.component.css',
})
export class CostcenterListItemComponent {
  @Input() costCenter: CostCenter;
}
