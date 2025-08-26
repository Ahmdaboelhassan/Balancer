import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { AccountSelectList } from '../../../Interfaces/Response/AccountSelectList';
import { AccountService } from '../../../Services/account.service';
import { SettingsService } from '../../../Services/settings.service';
import { ToastrService } from 'ngx-toastr';
import { TabulatorFull as Tabulator } from 'tabulator-tables';

@Component({
  selector: 'app-account-budget-settings',
  imports: [ReactiveFormsModule],
  templateUrl: './account-budget-settings.component.html',
  styleUrl: './account-budget-settings.component.css',
})
export class AccountBudgetSettingsComponent implements OnInit {
  @ViewChild('budgetAccountsTable') tableRef!: ElementRef;
  budgetAccountsTabulator!: Tabulator;
  accounts: AccountSelectList[] = [];

  constructor(
    private accountService: AccountService,
    private settingsService: SettingsService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.accountService.GetAllAccountSelectList().subscribe({
      next: (accounts) => {
        this.accounts = accounts;
        this.loadBudgetSettings();
      },
    });
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
          mutator: (value) => value || 0,
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
  }

  private loadBudgetSettings() {
    this.settingsService.getBudgetAccountSettings().subscribe({
      next: (settings) => {
        this.initBudgetAccountsTabulator(settings);
      },
    });
  }

  onSubmit() {
    const budgetSettings = this.budgetAccountsTabulator
      .getData()
      .filter((row) => row.accountId);

    if (budgetSettings.length > 0) {
      this.settingsService.editBudgetAccountSettings(budgetSettings).subscribe({
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
