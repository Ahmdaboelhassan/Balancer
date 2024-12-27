import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CostcenterListComponent } from './costcenter-list/costcenter-list.component';
import { CostCenter } from '../../Interfaces/Response/CostCenter';
import { CostcenterService } from '../../Services/costcenter.service';

@Component({
  selector: 'app-costcenter',
  imports: [RouterLink, CostcenterListComponent],
  templateUrl: './costcenter.component.html',
  styleUrl: './costcenter.component.css',
})
export class CostcenterComponent implements OnInit {
  costCenterService = inject(CostcenterService);
  costCenters: CostCenter[] = [];

  ngOnInit(): void {
    this.costCenterService.GetAllCostCenter().subscribe({
      next: (centers) => (this.costCenters = centers),
    });
  }
}
