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
import { DashboardSettingsDTO } from '../../Interfaces/Both/DashboardSettingsDTO';
import { ToastrService } from 'ngx-toastr';
import { RouterLink } from '@angular/router';
import { NgSelectComponent } from '@ng-select/ng-select';

@Component({
  selector: 'app-dashboard-settings',
  imports: [ReactiveFormsModule, RouterLink, NgSelectComponent],
  templateUrl: './dashboard-settings.component.html',
  styleUrl: './dashboard-settings.component.css',
})
export class DashboardSettingsComponent implements OnInit {
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

    this.settingsService.getDashboardSettings().subscribe({
      next: (settings) => {
        this.initForm(settings);
      },
    });
  }

  initForm(ds: DashboardSettingsDTO) {
    this.settingsForm = this.fb.group({
      id: [ds.id],
      account1: [ds.account1 ?? 0, [Validators.required]],
      account2: [ds.account2 ?? 0, [Validators.required]],
      account3: [ds.account3 ?? 0, [Validators.required]],
      account4: [ds.account4 ?? 0, [Validators.required]],
      otherExpensesTarget: [ds.otherExpensesTarget, [Validators.required]],
      addOnExpensesTarget: [ds.addOnExpensesTarget, [Validators.required]],
      applyOverBudgetToFunds: [ds.applyOverBudgetToFunds],
    });
  }

  onSubmit() {
    if (this.settingsForm.valid) {
      this.settingsService
        .editDashboardSettings(this.settingsForm.value)
        .subscribe({
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
