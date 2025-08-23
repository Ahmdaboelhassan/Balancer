import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TabulatorFull as Tabulator } from 'tabulator-tables';
import { Evaluation } from '../../../Interfaces/Response/Evaluation';
import { EvaluationService } from '../../../Services/evaluation.service';
import { Title } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../../../Services/account.service';
import { AccountSelectList } from '../../../Interfaces/Response/AccountSelectList';

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
    private toastr: ToastrService,
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
            this.titleService.setTitle('Edit Period');
            this.getEvaluation(+params['id']);
          } else {
            this.isEdit = false;
            this.titleService.setTitle('Create Period');
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
      from: [this.getFormattedDate(evaluation.from)],
      to: [this.getFormattedDate(evaluation.to)],
      name: [evaluation.name],
      profit: [evaluation.profit],
      profitPercentage: [evaluation.profitPercentage],
      income: [evaluation.income],
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
        height: '400px',
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
          this.toastr.success(response.message),
            this.router.navigate(['/Evaluation', 'Edit', createModel.id]);
        },
        error: (error) => this.toastr.error(error.error.message),
      });
    } else {
      this.evaluationService.Create(createModel).subscribe({
        next: (response) => {
          this.toastr.success(response.message);
          this.router.navigate(['/Evaluation', 'List']);
        },
        error: (error) => this.toastr.error(error.error.message),
      });
    }
  }
  Delete() {
    var result = confirm('Are You Sure For Deleting This Evaluation ?');
    if (result) {
      const id = this.evaluationForm.get('id').value;
      this.evaluationService.Delete(id).subscribe({
        next: (response) => {
          this.toastr.success(response.message);
          this.router.navigate(['/Evaluation', 'List']);
        },
        error: (error) => this.toastr.error(error.error.message),
      });
    }
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
        this.toastr.success(result.message);
      },
      error: (error) => this.toastr.error(error.error.message),
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
