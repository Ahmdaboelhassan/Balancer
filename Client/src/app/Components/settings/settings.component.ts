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

@Component({
  selector: 'app-settings',
  imports: [ReactiveFormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css',
})
export class SettingsComponent implements OnInit {
  settingsForm!: FormGroup;
  accounts: AccountSelectList[];

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private settingsService: SettingsService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.accountService
      .GetAllAccountSelectList()
      .subscribe({ next: (accounts) => (this.accounts = accounts) });

    this.settingsService.getSettings().subscribe({
      next: (settings) => {
        this.initForm(settings);
      },
    });
  }

  initForm(settings: any) {
    this.settingsForm = this.fb.group({
      id: [settings.id],
      defaultCreditAccount: [settings.defaultCreditAccount],
      defaultDebitAccount: [settings.defaultDebitAccount],
      defaultPeriodDays: [settings.defaultPeriodDays ?? 7],
      defaultDayRate: [settings.defaultDayRate, [Validators.required]],
      expensesAccount: [settings.expensesAccount],
      revenueAccount: [settings.revenueAccount],
      assetsAccount: [settings.assetsAccount],
      currentAssetsAccount: [settings.currentAssetsAccount],
      fixedAssetsAccount: [settings.fixedAssetsAccount],
      currentCashAccount: [settings.currentCashAccount],
      liabilitiesAccount: [settings.liabilitiesAccount],
      banksAccount: [settings.banksAccount],
      drawersAccount: [settings.drawersAccount],
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
      this.settingsService.editSettings(this.settingsForm.value).subscribe({
        next: (result) => {
          this.toastr.success(result.message);
        },
        error: (err) => {
          this.toastr.error(err.error.message);
        },
      });
    }
  }
}
