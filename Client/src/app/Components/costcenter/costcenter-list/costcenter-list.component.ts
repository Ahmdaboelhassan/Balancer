import { Component, Input } from '@angular/core';
import { CostCenter } from '../../../Interfaces/Response/CostCenter';
import { CostcenterListItemComponent } from './costcenter-list-item/costcenter-list-item.component';

@Component({
  selector: 'app-costcenter-list',
  imports: [CostcenterListItemComponent],
  templateUrl: './costcenter-list.component.html',
  styleUrl: './costcenter-list.component.css',
})
export class CostcenterListComponent {
  @Input() costCenters: CostCenter[];
}
