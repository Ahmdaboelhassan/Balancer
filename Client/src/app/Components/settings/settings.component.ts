import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AccountService } from '../../Services/account.service';
import { SettingsService } from '../../Services/settings.service';
import { AccountSelectList } from '../../Interfaces/Response/AccountSelectList';
import { ToastrService } from 'ngx-toastr';
import { NgSelectComponent } from '@ng-select/ng-select';
import { CostCenterSelectList } from '../../Interfaces/Response/CostCenterSelectList';
import { CostcenterService } from '../../Services/costcenter.service';
import { Title } from '@angular/platform-browser';
import { NgFor } from '@angular/common';

@Component({
  selector: 'app-settings',
  imports: [ReactiveFormsModule, NgSelectComponent, NgFor],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css',
})
export class SettingsComponent implements OnInit {
  settingsForm!: FormGroup;
  accounts: AccountSelectList[];
  costCenters: CostCenterSelectList[];

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private costCenterService: CostcenterService,
    private settingsService: SettingsService,
    private toastr: ToastrService,
    private titleServive: Title,
  ) {}

  ngOnInit() {
    this.accountService
      .GetAllAccountSelectList()
      .subscribe({ next: (accounts) => (this.accounts = accounts) });

    this.costCenterService
      .GetAllCostCenterSelectList()
      .subscribe({ next: (costCenters) => (this.costCenters = costCenters) });

    this.settingsService.getSettings().subscribe({
      next: (settings) => {
        this.initForm(settings);
      },
    });

    this.titleServive.setTitle('Settings');
  }

  initForm(settings: any) {
    this.settingsForm = this.fb.group({
      id: [settings.id],
      defaultCreditAccount: [settings.defaultCreditAccount ?? ''],
      defaultDebitAccount: [settings.defaultDebitAccount ?? ''],
      defaultPeriodDays: [settings.defaultPeriodDays ?? 7],
      defaultDayRate: [settings.defaultDayRate, [Validators.required]],
      expensesAccount: [settings.expensesAccount ?? ''],
      revenueAccount: [settings.revenueAccount ?? ''],
      assetsAccount: [settings.assetsAccount ?? ''],
      currentAssetsAccount: [settings.currentAssetsAccount ?? ''],
      fixedAssetsAccount: [settings.fixedAssetsAccount ?? ''],
      currentCashAccount: [settings.currentCashAccount ?? ''],
      liabilitiesAccount: [settings.liabilitiesAccount ?? ''],
      notBudgetCostCenter: [settings.notBudgetCostCenter ?? ''],
      banksAccount: [settings.banksAccount ?? ''],
      drawersAccount: [settings.drawersAccount ?? ''],
      investmentAccount: [settings.investmentAccount ?? ''],
      levelOneDigits: [settings.levelOneDigits, [Validators.required]],
      levelTwoDigits: [settings.levelTwoDigits, [Validators.required]],
      levelThreeDigits: [settings.levelThreeDigits, [Validators.required]],
      levelFourDigits: [settings.levelFourDigits, [Validators.required]],
      levelFiveDigits: [settings.levelFiveDigits, [Validators.required]],
      maxAccountLevel: [settings.maxAccountLevel, [Validators.required]],
    });
  }

  onSubmit() {
    if (this.settingsForm.valid) {
      // clone the form value
      const payload = { ...this.settingsForm.value };

      // loop through keys
      Object.keys(payload).forEach((key) => {
        if (payload[key] === '' || payload[key] === undefined) {
          payload[key] = null;
        }
      });

      this.settingsService.editSettings(payload).subscribe({
        next: (result) => this.toastr.success(result.message),
        error: (err) => this.toastr.error(err.error.message),
      });
    }
  }
}
