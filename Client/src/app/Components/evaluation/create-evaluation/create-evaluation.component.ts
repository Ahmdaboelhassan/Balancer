import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TabulatorFull as Tabulator } from 'tabulator-tables';
import { Evaluation } from '../../../Interfaces/Response/Evaluation';
import { EvaluationService } from '../../../Services/evaluation.service';
import { Title } from '@angular/platform-browser';
import { AccountService } from '../../../Services/account.service';
import { AccountSelectList } from '../../../Interfaces/Response/AccountSelectList';
import Swal from 'sweetalert2';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-create-evaluation',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './create-evaluation.component.html',
  styleUrl: './create-evaluation.component.css',
})
export class CreateEvaluationComponent implements OnInit {
  @ViewChild('evaluationDetails') tableRef!: ElementRef;
  evaluationDetailsTabulator!: Tabulator;
  isEdit = false;
  evaluationForm!: FormGroup;
  accounts: AccountSelectList[];

  constructor(
    private evaluationService: EvaluationService,
    private accountService: AccountService,
    private titleService: Title,
    private route: ActivatedRoute,
    private router: Router,
    private fb: FormBuilder
  ) {}

  ngOnInit(): void {
    this.accountService
      .GetAllAccountSelectList()
      .subscribe({
        next: (accounts) => (this.accounts = accounts),
      })
      .add(() => {
        this.route.params.subscribe((params) => {
          if (params['id']) {
            this.isEdit = true;
            this.titleService.setTitle('Edit Evaluation');
            this.getEvaluation(+params['id']);
          } else {
            this.isEdit = false;
            this.titleService.setTitle('Create Evaluation');
            this.getEvaluation();
          }
        });
      });
  }

  getEvaluation(id?: number) {
    if (id) {
      this.evaluationService.Get(id).subscribe((evaluation) => {
        this.initEvaluationForm(evaluation);
        this.initEvaluationDetailsTabulator(evaluation.evaluationDetails);
      });
    } else {
      this.evaluationService.New().subscribe((evaluation) => {
        this.initEvaluationForm(evaluation);
        this.initEvaluationDetailsTabulator(evaluation.evaluationDetails);
      });
    }
  }

  initEvaluationForm(evaluation: Evaluation) {
    this.evaluationForm = this.fb.group({
      id: [evaluation.id],
      from: [this.getFormattedDate(evaluation.from), Validators.required],
      to: [this.getFormattedDate(evaluation.to), Validators.required],
      name: [evaluation.name, Validators.required],
      profit: [evaluation.profit, Validators.required],
      profitPercentage: [evaluation.profitPercentage],
      income: [evaluation.income, [Validators.required, Validators.min(1)]],
      note: [evaluation.note],
      lastUpdatedAt: [{ value: evaluation.lastUpdatedAt, disabled: true }],
      createdAt: [{ value: evaluation.createdAt, disabled: true }],
    });
  }

  initEvaluationDetailsTabulator(details) {
    const accountOptions = this.accounts.map((acc) => ({
      value: acc.id,
      label: acc.name,
    }));
    const data = details && details.length > 0 ? details : [{}];

    this.evaluationDetailsTabulator = new Tabulator(
      this.tableRef.nativeElement,
      {
        height: '450px',
        layout: 'fitColumns',
        columns: [
          {
            title: 'Account',
            field: 'accountId',
            editor: 'list',
            editorParams: {
              values: accountOptions,
              autocomplete: true,
              listOnEmpty: true,
              allowEmpty: false,
            },
            formatter: (cell) => {
              const account = this.accounts.find(
                (a) => a.id === cell.getValue()
              );
              return account ? account.name : '';
            },
            cellEdited: (cell) => {
              const row = cell.getRow();
              row.update({ amount: 0, percentage: 0 });
            },
          },
          {
            title: 'Amount',
            field: 'amount',
            editor: 'number',
            formatter: 'money',
            formatterParams: {
              precision: 2,
            },
            bottomCalc: 'sum',
            bottomCalcFormatter: 'money',
            bottomCalcFormatterParams: { precision: 2 },
            mutator: (value) => value ?? 0,
            cellEdited: (cell) => {
              const table = cell.getTable();
              const data = table.getData();
              let income = Number(this.evaluationForm.get('income').value);
              income = income == 0 ? 1 : income;

              let totalAmount = 0;
              let newData = [];

              data.forEach((e) => {
                newData.push({
                  accountId: e.accountId,
                  amount: e.amount,
                  percentage: Math.round((e.amount / income) * 100),
                });
                totalAmount += e.amount;
              });

              table.setData(newData);
              table.setSort('amount', 'desc');

              const profit = Math.round(income - totalAmount);
              this.evaluationForm.patchValue({
                profit: profit,
                profitPercentage: Math.round((profit / income) * 100),
              });
            },
          },
          {
            title: 'Percentage',
            field: 'percentage',
            bottomCalc: 'sum',
            bottomCalcFormatter: 'money',
            bottomCalcFormatterParams: {
              symbol: '%',
              symbolAfter: true,
              precision: 2,
            },
            formatter: 'money',
            formatterParams: {
              symbol: '%',
              symbolAfter: true,
              precision: 2,
            },
            mutator: (value) => value || 0,
          },
        ],
        data: data,
        rowContextMenu: [
          {
            label:
              "<i class='fas fa-plus-circle text-green-600'></i> Add New Row",
            action: (e, row) => {
              row.getTable().addRow();
            },
          },
          {
            label:
              "<i class='fas fa-trash-alt text-red-600'></i> Delete Current Row",
            action: (e, row) => {
              row.delete();

              const table = row.getTable();
              if (table.getData().length === 0) {
                table.addRow({});
              }
            },
          },
          {
            label: "<i class='fas fa-trash text-red-600'></i> Delete All Rows",
            action: (e, row) => {
              const table = row.getTable();
              table.clearData();
              table.addRow();
            },
          },
        ],
      }
    );
  }

  Submit() {
    const formValue = this.evaluationForm.value;

    const createModel: Evaluation = {
      id: formValue.id,
      from: formValue.from,
      to: formValue.to,
      note: formValue.note,
      name: formValue.name,
      profit: formValue.profit,
      profitPercentage: formValue.profitPercentage,
      income: formValue.income,
      evaluationDetails: this.evaluationDetailsTabulator
        .getData()
        .filter((a) => a.accountId),
    };

    if (this.isEdit) {
      this.evaluationService.Edit(createModel).subscribe({
        next: (response) => {
          Swal.fire({
            title: 'Edit Evaluation',
            text: response.message,
            icon: 'success',
            showConfirmButton: false,
            timer: environment.sweetAlertTimeOut,
          }).then(() => {
            this.router.navigate(['/Evaluation', 'Edit', createModel.id]);
          });
        },
        error: (error) => {
          Swal.fire({
            title: 'Edit Evaluation',
            text: error.error.message,
            icon: 'error',
            showConfirmButton: true,
          });
        },
      });
    } else {
      this.evaluationService.Create(createModel).subscribe({
        next: (response) => {
          Swal.fire({
            title: 'Create Evaluation',
            text: response.message,
            icon: 'success',
            showConfirmButton: false,
            timer: environment.sweetAlertTimeOut,
          }).then(() => {
            this.router.navigate(['/Evaluation', 'List']);
          });
        },
        error: (error) => {
          Swal.fire({
            title: 'Create Evaluation',
            text: error.error.message,
            icon: 'error',
            showConfirmButton: true,
          });
        },
      });
    }
  }
  Delete() {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Are you sure you want to delete this evaluation?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel',
    }).then((result) => {
      if (result.isConfirmed) {
        const id = this.evaluationForm.get('id')?.value;
        this.evaluationService.Delete(id).subscribe({
          next: (response) => {
            Swal.fire({
              title: 'Delete Evaluation',
              text: response.message,
              icon: 'success',
              showConfirmButton: false,
              timer: environment.sweetAlertTimeOut,
            }).then(() => {
              this.router.navigate(['/Evaluation', 'List']);
            });
          },
          error: (error) => {
            Swal.fire({
              title: 'Delete Evaluation',
              text: error.error.message,
              icon: 'error',
              showConfirmButton: true,
            });
          },
        });
      }
    });
  }

  CalualateDetails() {
    const formValue = this.evaluationForm.value;

    const createModel: Evaluation = {
      id: formValue.id,
      from: formValue.from,
      to: formValue.to,
      note: formValue.note,
      name: formValue.name,
      profit: formValue.profit,
      profitPercentage: formValue.profitPercentage,
      income: formValue.income,
      evaluationDetails: this.evaluationDetailsTabulator
        .getData()
        .filter((a) => a.accountId),
    };

    this.evaluationService.CalculateDetailsBalances(createModel).subscribe({
      next: (result) => {
        this.initEvaluationForm(result.data);
        this.evaluationDetailsTabulator.setData(result.data?.evaluationDetails);

        Swal.fire({
          title: 'Calculation Completed',
          text: result.message,
          icon: 'success',
          showConfirmButton: false,
          timer: environment.sweetAlertTimeOut,
        });
      },
      error: (error) => {
        Swal.fire({
          title: 'Calculation Failed',
          text: error.error.message,
          icon: 'error',
          showConfirmButton: true,
        });
      },
    });
  }

  getFormattedDate(date: string) {
    return new Date(date).toLocaleDateString('en-CA');
  }

  calculateProfitPercentage() {
    const profit = Number(this.evaluationForm.get('profit').value);
    const income = Number(this.evaluationForm.get('income').value);

    const profitPercentage = Math.round((profit / income) * 100);

    this.evaluationForm.patchValue({ profitPercentage: profitPercentage });
  }
  calculateProfit() {
    const income = Number(this.evaluationForm.get('income').value);
    const totalAmount = this.evaluationDetailsTabulator
      .getData()
      .reduce((acc, e) => acc + e.amount, 0);
    const profit = income - totalAmount;
    const profitPercentage = Math.round((profit / income) * 100);
    this.evaluationForm.patchValue({
      profitPercentage: profitPercentage,
      profit: profit,
    });
  }
}
