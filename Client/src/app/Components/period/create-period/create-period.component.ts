import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { PeriodService } from '../../../Services/period.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Period } from '../../../Interfaces/Response/Period';
import { finalize } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { CreatePeriod } from '../../../Interfaces/Request/CreatePeriod';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-create-period',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-period.component.html',
  styleUrl: './create-period.component.css',
})
export class CreatePeriodComponent implements OnInit {
  isEdit = false;
  periodForm!: FormGroup;
  period: Period | any;

  constructor(
    private titleService: Title,
    private periodService: PeriodService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      if (params['id']) {
        this.isEdit = true;
        this.getPeriod(+params['id']);
      } else {
        this.getPeriod();
      }
    });

    if (this.isEdit) {
      this.titleService.setTitle('Edit Period');
    } else {
      this.titleService.setTitle('Create Period');
    }
  }

  getPeriod(id?: number) {
    if (id) {
      this.periodService
        .GetPeriodDetail(id)
        .pipe(finalize(() => this.intializeForm()))
        .subscribe((period) => {
          this.period = period;
        });
    } else {
      this.periodService
        .GetCreatePeriod()
        .pipe(finalize(() => this.intializeForm()))
        .subscribe((period) => {
          this.period = period;
        });
    }
  }

  intializeForm() {
    this.periodForm = new FormGroup({
      id: new FormControl(this.period.id),
      name: new FormControl(this.period.name),
      totalAmount: new FormControl({
        value: this.period.totalAmount,
        disabled: true,
      }),
      from: new FormControl(this.parseDate(this.period.from)),
      to: new FormControl(this.parseDate(this.period.to)),
      createdAt: new FormControl({
        value: this.period.createdAt,
        disabled: true,
      }),
      lastUpdatedAt: new FormControl({
        value: this.period.lastUpdatedAt,
        disabled: true,
      }),
      daysCount: new FormControl(this.period?.daysCount),
      notes: new FormControl(this.period.notes),
    });
  }

  parseDate(date: string) {
    if (date) {
      return date.split('T')[0];
    }
    return '';
  }

  Submit() {
    const formValue = this.periodForm.value;
    const createModel: CreatePeriod = {
      id: formValue.id,
      from: formValue.from,
      to: formValue.to,
      daysCount: formValue.daysCount,
      notes: formValue.notes,
    };

    if (this.isEdit) {
      this.periodService.EditPeriod(createModel).subscribe({
        next: (response) => {
          Swal.fire({
            title: 'Edit Period',
            text: response.message,
            icon: 'success',
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true,
          });
        },
        error: (error) => {
          Swal.fire({
            title: 'Edit Period',
            text: error.error.message,
            icon: 'error',
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true,
          });
        },
      });
    } else {
      this.periodService.CreatePeriod(createModel).subscribe({
        next: (response) => {
          Swal.fire({
            title: 'Create Period',
            text: response.message,
            icon: 'success',
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true,
          }).then(() => {
            this.router.navigate(['/Period', 'List']);
          });
        },
        error: (error) => {
          Swal.fire({
            title: 'Create Period',
            text: error.error.message,
            icon: 'error',
            showConfirmButton: false,
            timer: 2000,
            timerProgressBar: true,
          });
        },
      });
    }
  }

  DeletePeriod() {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Are you sure you want to delete this period?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel',
    }).then((result) => {
      if (result.isConfirmed) {
        this.periodService.DeletePeriod(this.period.id).subscribe({
          next: (response) => {
            Swal.fire({
              title: 'Delete Period',
              text: response.message,
              icon: 'success',
              showConfirmButton: false,
              timer: 2000,
              timerProgressBar: true,
            }).then(() => {
              this.router.navigate(['/Period', 'List']);
            });
          },
          error: (error) => {
            Swal.fire({
              title: 'Delete Period',
              text: error.error.message,
              icon: 'error',
              showConfirmButton: false,
              timer: 2000,
              timerProgressBar: true,
            });
          },
        });
      }
    });
  }
}
