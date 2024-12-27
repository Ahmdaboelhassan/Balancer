import { Component } from '@angular/core';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { CostCenter } from '../../../Interfaces/Response/CostCenter';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CostcenterService } from '../../../Services/costcenter.service';
import { Title } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';
import { CreateCostCenter } from '../../../Interfaces/Request/CreateCostCenter';

@Component({
  selector: 'app-create-costcenter',
  imports: [RouterLink, ReactiveFormsModule],
  templateUrl: './create-costcenter.component.html',
  styleUrl: './create-costcenter.component.css',
})
export class CreateCostcenterComponent {
  isEdit = false;
  CostCenterForm: FormGroup;
  CostCenter: CostCenter;
  constructor(
    private route: ActivatedRoute,
    private CostCenterService: CostcenterService,
    private titleService: Title,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      if (params['id']) {
        this.isEdit = true;
        this.getCostCenter(+params['id']);
        this.titleService.setTitle('Edit CostCenter');
      } else {
        this.intializeCreateForm();
        this.titleService.setTitle('Create CostCenter');
      }
    });
  }

  getCostCenter(id) {
    this.CostCenterService.GetCostCenter(id).subscribe({
      next: (CostCenter) => {
        {
          this.CostCenter = CostCenter;
          this.intializeEditForm();
        }
      },
    });
  }

  intializeEditForm() {
    this.CostCenterForm = new FormGroup({
      id: new FormControl(this.CostCenter.id),
      name: new FormControl(this.CostCenter.name, Validators.required),
      description: new FormControl(this.CostCenter.description),
      CreatedAt: new FormControl({
        value: this.CostCenter.createdAt,
        disabled: true,
      }),
    });
  }
  intializeCreateForm() {
    this.CostCenterForm = new FormGroup({
      id: new FormControl(0),
      name: new FormControl('', Validators.required),
      description: new FormControl(''),
      CreatedAt: new FormControl({
        value: '',
        disabled: true,
      }),
    });
  }
  Submit() {
    if (!this.CostCenterForm.valid) {
      this.toastr.error('From Is Invalid !');
      return;
    }
    const formValue = this.CostCenterForm.value;
    const SaveJournal: CreateCostCenter = {
      id: formValue.id,
      name: formValue.name,
      description: formValue.description,
    };

    if (this.isEdit) {
      this.CostCenterService.EditCostCenter(SaveJournal).subscribe({
        next: (result) => {
          this.toastr.success(result.message);
          this.router.navigate(['/CostCenter', 'List']);
        },
        error: (error) => {
          this.toastr.error(error.error.message);
        },
      });
    } else {
      this.CostCenterService.CreateCostCenter(SaveJournal).subscribe({
        next: (result) => {
          this.toastr.success(result.message);
          this.router.navigate(['/CostCenter', 'List']);
        },
        error: (error) => this.toastr.error(error.error.message),
      });
    }
  }
  DeleteCostCenter() {
    this.CostCenterService.DeleteCostCenter(this.CostCenter.id).subscribe({
      next: (result) => {
        this.toastr.success(result.message);
        this.router.navigate(['/CostCenter', 'List']);
      },
      error: (error) => this.toastr.error(error.error.message),
    });
  }
}
