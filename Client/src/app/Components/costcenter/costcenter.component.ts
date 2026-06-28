import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { CostcenterListComponent } from './costcenter-list/costcenter-list.component';
import { CostCenter } from '../../Interfaces/Response/CostCenter';
import { CostcenterService } from '../../Services/costcenter.service';
import { switchMap } from 'rxjs';

@Component({
  selector: 'app-costcenter',
  imports: [RouterLink, CostcenterListComponent],
  templateUrl: './costcenter.component.html',
  styleUrl: './costcenter.component.css',
})
export class CostcenterComponent implements OnInit {
  costCenterService = inject(CostcenterService);
  costCenters: CostCenter[] = [];
  route = inject(ActivatedRoute);

  ngOnInit(): void {
    this.route.queryParamMap
      .pipe(
        switchMap((params) => {
          const archive = params.get('archive') === 'true';
          return this.costCenterService.GetAllCostCenter(archive);
        }),
      )
      .subscribe({
        next: (centers) => (this.costCenters = centers),
      });
  }
}
