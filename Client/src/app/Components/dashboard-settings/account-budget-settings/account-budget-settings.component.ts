import { Component } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { AccountSelectList } from '../../../Interfaces/Response/AccountSelectList';
import { AccountService } from '../../../Services/account.service';
import { SettingsService } from '../../../Services/settings.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-account-budget-settings',
  imports: [ReactiveFormsModule],
  templateUrl: './account-budget-settings.component.html',
  styleUrl: './account-budget-settings.component.css',
})
export class AccountBudgetSettingsComponent {
  form!: FormGroup;
  accounts: AccountSelectList[] = [];

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private settingsService: SettingsService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.initForm();
    this.accountService.GetAllAccountSelectList().subscribe({
      next: (accounts) => (this.accounts = accounts),
    });

    this.loadBudgetSettings();
  }

  private initForm() {
    this.form = this.fb.group({
      budgetAccounts: this.fb.array([]),
    });
  }

  get budgetAccounts() {
    return this.form.get('budgetAccounts') as FormArray;
  }

  private createBudgetAccount() {
    return this.fb.group({
      id: [0],
      accountId: ['', Validators.required],
      budget: [0, [Validators.required, Validators.min(0)]],
      color: ['#000000', Validators.required],
      displayName: ['', Validators.required],
    });
  }

  private loadBudgetSettings() {
    this.settingsService.getBudgetAccountSettings().subscribe({
      next: (settings) => {
        if (settings && settings.length > 0) {
          settings.forEach((setting) => {
            const budgetGroup = this.createBudgetAccount();
            budgetGroup.patchValue(setting);
            this.budgetAccounts.push(budgetGroup);
          });
        }
      },
    });
  }

  addBudgetAccount() {
    this.budgetAccounts.push(this.createBudgetAccount());
  }

  removeBudgetAccount(index: number) {
    this.budgetAccounts.removeAt(index);
  }

  onSubmit() {
    if (this.form.valid) {
      this.settingsService
        .editBudgetAccountSettings(this.budgetAccounts.value)
        .subscribe({
          next: (response) => {
            this.toastr.success(response.message);
          },
          error: (err) => {
            this.toastr.error(err.error.message);
          },
        });
    }
  }
}
