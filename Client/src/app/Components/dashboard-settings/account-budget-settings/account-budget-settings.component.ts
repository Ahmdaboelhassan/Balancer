import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { forkJoin } from 'rxjs';
import { DashboardSettingsDTO } from '../../../Interfaces/Both/DashboardSettingsDTO';
import { AccountSelectList } from '../../../Interfaces/Response/AccountSelectList';
import { AccountService } from '../../../Services/account.service';
import { SettingsService } from '../../../Services/settings.service';
import { ToastrService } from 'ngx-toastr';
import { TabulatorFull as Tabulator } from 'tabulator-tables';
import { NgSelectComponent } from '@ng-select/ng-select';
import { PaidMethodOptions, PaidMethods } from '../../../Enums/PaidMethods';
import {
  BudgetAccountTypeOptions,
  BudgetAccountTypes,
} from '../../../Enums/BudgetAccountTypes';

@Component({
  selector: 'app-account-budget-settings',
  imports: [ReactiveFormsModule, NgSelectComponent],
  templateUrl: './account-budget-settings.component.html',
  styleUrl: './account-budget-settings.component.css',
})
export class AccountBudgetSettingsComponent implements OnInit {
  @ViewChild('budgetAccountsTable') tableRef!: ElementRef;
  budgetAccountsTabulator!: Tabulator;
  accounts: AccountSelectList[] = [];
  budgetSettingsForm!: FormGroup;
  // Total Revenue follows Total Income - (table budgets + other expenses + add on expenses) until the user edits it
  isTotalRevenueAuto = true;
  private tableBudgetTotal = 0;

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private settingsService: SettingsService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.budgetSettingsForm = this.fb.group({
      otherExpensesTarget: [0, [Validators.required]],
      addOnExpensesTarget: [0, [Validators.required]],
      totalRevenue: [0, [Validators.required]],
      totalIncome: [0, [Validators.required]],
      savingTargetAccountId: [null],
      savingTargetDisplayName: [''],
      applyOverBudgetToFunds: [false],
    });

    this.budgetSettingsForm
      .get('otherExpensesTarget')!
      .valueChanges.subscribe(() => this.updateTotalRevenue());
    this.budgetSettingsForm
      .get('addOnExpensesTarget')!
      .valueChanges.subscribe(() => this.updateTotalRevenue());
    this.budgetSettingsForm
      .get('totalIncome')!
      .valueChanges.subscribe(() => this.updateTotalRevenue());

    forkJoin([
      this.accountService.GetAllAccountSelectList(),
      this.settingsService.getBudgetAccountSettings(),
      this.settingsService.getDashboardSettings(),
    ]).subscribe({
      next: ([accounts, budgetAccounts, settings]: [
        AccountSelectList[],
        any[],
        DashboardSettingsDTO,
      ]) => {
        this.accounts = accounts;
        this.tableBudgetTotal = this.sumBudgets(budgetAccounts);

        this.budgetSettingsForm.patchValue(
          {
            otherExpensesTarget: settings.otherExpensesTarget ?? 0,
            addOnExpensesTarget: settings.addOnExpensesTarget ?? 0,
            totalRevenue: settings.totalRevenue ?? 0,
            totalIncome: settings.totalIncome ?? 0,
            savingTargetAccountId: settings.savingTargetAccountId ?? null,
            savingTargetDisplayName: settings.savingTargetDisplayName ?? '',
            applyOverBudgetToFunds: settings.applyOverBudgetToFunds ?? false,
          },
          { emitEvent: false },
        );

        // Keep auto mode when nothing was saved yet or the saved value still equals the calculation
        this.isTotalRevenueAuto =
          !settings.totalRevenue ||
          settings.totalRevenue === this.calculateTotalRevenue();
        this.updateTotalRevenue();

        this.initBudgetAccountsTabulator(budgetAccounts);
      },
    });
  }

  onTotalRevenueInput() {
    this.isTotalRevenueAuto = false;
  }

  resetTotalRevenue() {
    this.isTotalRevenueAuto = true;
    this.updateTotalRevenue();
  }

  private updateTotalRevenue() {
    if (!this.isTotalRevenueAuto) return;

    this.budgetSettingsForm.patchValue(
      { totalRevenue: this.calculateTotalRevenue() },
      { emitEvent: false },
    );
  }

  private calculateTotalRevenue(): number {
    const { otherExpensesTarget, addOnExpensesTarget, totalIncome } =
      this.budgetSettingsForm.getRawValue();
    const expenses =
      this.tableBudgetTotal +
      (Number(otherExpensesTarget) || 0) +
      (Number(addOnExpensesTarget) || 0);

    return Math.round(((Number(totalIncome) || 0) - expenses) * 100) / 100;
  }

  // Only rows with an account are saved, so only they count toward the total
  private sumBudgets(rows: any[]): number {
    return rows
      .filter((row) => row.accountId)
      .reduce((sum, row) => sum + (Number(row.budget) || 0), 0);
  }

  private initBudgetAccountsTabulator(data: any[] = []) {
    const accountOptions = this.accounts.map((acc) => ({
      value: acc.id,
      label: acc.name,
    }));

    this.budgetAccountsTabulator = new Tabulator(this.tableRef.nativeElement, {
      movableRows: true,
      tabEndNewRow: true,
      layout: 'fitColumns',
      data: data.length > 0 ? data : [{}],
      columns: [
        {
          title: '',
          field: 'color',
          editor: 'input',
          width: 70,
          editorParams: {
            elementAttributes: {
              type: 'color',
            },
          },
          formatter: (cell) => {
            const value = cell.getValue() || '#000000';
            return `<div class="flex items-center gap-2">
                      <div class="w-6 h-6 rounded-full" style="background-color: ${value}"></div>
                     
                    </div>`;
          },
          mutator: (value) => value || '#000000',
        },
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
            const account = this.accounts.find((a) => a.id === cell.getValue());
            return account ? account.name : '';
          },
        },
        {
          title: 'Display Name',
          field: 'displayName',
          editor: 'input',
        },
         {
          title: 'Budget',
          field: 'budget',
          editor: 'number',
          editorParams: {
            min: 0,
          },
          formatter: 'money',
          formatterParams: {
            precision: 2,
          },
          bottomCalc: 'sum',
          bottomCalcFormatter: 'money',
          bottomCalcFormatterParams: { precision: 2 },
          mutator: (value) => value || 0,
        },
        {
          title: 'Type',
          field: 'typeId',
          editor: 'list',
          editorParams: {
            values: BudgetAccountTypeOptions,
            allowEmpty: false,
          },
          formatter: (cell) =>
            BudgetAccountTypeOptions.find((o) => o.value === cell.getValue())
              ?.label ?? '',
          mutator: (value) => value || BudgetAccountTypes.PersonalPriority,
        },
        {
          title: 'Paid Method',
          field: 'paidMethodId',
          editor: 'list',
          editorParams: {
            values: PaidMethodOptions,
            allowEmpty: false,
          },
          formatter: (cell) =>
            PaidMethodOptions.find((o) => o.value === cell.getValue())
              ?.label ?? '',
          mutator: (value) => value || PaidMethods.Once,
        },
       
        {
          title: 'Best Budget',
          field: 'bestBudget',
          editor: 'number',
          editorParams: {
            min: 0,
          },
          formatter: 'money',
          formatterParams: {
            precision: 2,
          },
          bottomCalc: 'sum',
          bottomCalcFormatter: 'money',
          bottomCalcFormatterParams: { precision: 2 },
          mutator: (value) => value || 0,
        },
        {
          title: 'Maximum Budget',
          field: 'maximumBudget',
          editor: 'number',
          visible: false,
          editorParams: {
            min: 0,
          },
          formatter: 'money',
          formatterParams: {
            precision: 2,
          },
          bottomCalc: 'sum',
          bottomCalcFormatter: 'money',
          bottomCalcFormatterParams: { precision: 2 },
          mutator: (value) => value || 0,
        },
         {
          title: 'Notes',
          field: 'notes',
          editor: 'input',
        },
      ],
      rowContextMenu: [
        {
          label:
            "<i class='fas fa-plus-circle text-green-600'></i> Add New Row",
          action: (e, row) => {
            row.getTable().addRow({});
          },
        },
        {
          label: "<i class='fas fa-trash-alt text-red-600'></i> Delete Row",
          action: (e, row) => {
            row.delete();
            if (row.getTable().getData().length === 0) {
              row.getTable().addRow({});
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
    });

    // Fires on cell edits, added rows and deleted rows
    this.budgetAccountsTabulator.on('dataChanged', (rows) => {
      this.tableBudgetTotal = this.sumBudgets(rows);
      this.updateTotalRevenue();
    });
  }

  onSubmit() {
    const budgetSettings = this.budgetAccountsTabulator
      .getData()
      .filter((row) => row.accountId);

    if (budgetSettings.length > 0) {
      this.settingsService
        .editBudgetAccountSettings({
          ...this.budgetSettingsForm.value,
          budgetAccounts: budgetSettings,
        })
        .subscribe({
          next: (response) => {
            this.toastr.success(response.message);
          },
          error: (err) => {
            this.toastr.error(err.error.message);
          },
        });
    } else {
      this.toastr.warning('Please add at least one budget account setting');
    }
  }
}
