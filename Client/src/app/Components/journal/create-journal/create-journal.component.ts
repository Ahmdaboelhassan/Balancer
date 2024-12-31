import { Component, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { JournalService } from '../../../Services/journal.service';
import { Title } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Journal } from '../../../Interfaces/Response/Journal';
import { NgClass, NgFor } from '@angular/common';
import { AccountService } from '../../../Services/account.service';
import { CreateJournal } from '../../../Interfaces/Request/CreateJournal';

@Component({
  imports: [RouterLink, ReactiveFormsModule, NgFor, NgClass],
  templateUrl: './create-journal.component.html',
  styleUrl: './create-journal.component.css',
})
export class CreateJournalComponent {
  Journal: WritableSignal<Journal> | null = signal(null);
  JournalForm: FormGroup;
  creditBalance: number = 0;
  debitBalance: number = 0;
  journalAmount: number = 0;
  isEdit = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private journalService: JournalService,
    private accountService: AccountService,
    private titleService: Title,
    private toastr: ToastrService
  ) {
    this.titleService.setTitle('Create Journal');
    var JournalId = this.route.snapshot.params['id'];

    if (JournalId) {
      this.isEdit = true;
      this.titleService.setTitle('Edit Journal');
      this.journalService.GetEditJournal(JournalId).subscribe({
        next: (journal) => {
          this.Journal.set(journal);
          this.intializeForm(journal);
        },
      });
    } else {
      const periodId = this.route.snapshot.queryParams['periodId'] || 0;
      this.journalService.GetNewJournal(periodId).subscribe({
        next: (journal) => {
          this.Journal.set(journal);
          this.intializeForm(journal);
        },
      });
    }
  }

  intializeForm(journal: Journal) {
    this.JournalForm = new FormGroup({
      id: new FormControl(journal.id),
      details: new FormControl(journal.detail, Validators.required),
      amount: new FormControl(journal.amount, Validators.min(1)),
      debit: new FormControl(journal.debitAccountId),
      credit: new FormControl(journal.creditAccountId),
      costCenter: new FormControl(journal.costCenterId),
      code: new FormControl({ disabled: true, value: journal.code }),
      created: new FormControl(
        this.GetLocaleDateTime(new Date(journal.createdAt))
      ),
      lastUpdate: new FormControl({
        disabled: true,
        value: journal.lastUpdatedAt,
      }),
      description: new FormControl(journal.description),
      periodId: new FormControl(journal.periodId),
      creditBalance: new FormControl({ disabled: true, value: 0 }),
      debitBalance: new FormControl({ disabled: true, value: 0 }),
    });
    this.GetCreditBalance();
    this.GetDebitBalance();
  }

  GetCreditBalance() {
    const accountId = this.JournalForm.get('credit').value;
    this.accountService.GetAccountBalance(accountId).subscribe({
      next: (result) => {
        this.JournalForm.patchValue({ creditBalance: result.balance });
        this.creditBalance = result.balance;
        this.JournalForm.patchValue({
          creditBalance: this.creditBalance - this.journalAmount,
        });
      },
      error: (error) => this.toastr.error(error),
    });
  }
  GetDebitBalance() {
    const accountId = this.JournalForm.get('debit').value;
    this.accountService.GetAccountBalance(accountId).subscribe({
      next: (result) => {
        this.JournalForm.patchValue({ debitBalance: result.balance });
        this.debitBalance = result.balance;
        this.JournalForm.patchValue({
          debitBalance: this.debitBalance + this.journalAmount,
        });
      },
      error: (error) => this.toastr.error(error),
    });
  }
  ChangeBalances() {
    this.journalAmount = this.JournalForm.get('amount')?.value;
    this.JournalForm.patchValue({
      creditBalance: this.creditBalance - this.journalAmount,
    });
    this.JournalForm.patchValue({
      debitBalance: this.debitBalance + this.journalAmount,
    });
  }

  SaveJournal() {
    const form = this.JournalForm.value;
    const journal: CreateJournal = {
      id: form.id,
      amount: form.amount,
      detail: form.details,
      costCenterId: form.costCenter,
      creditAccountId: form.credit,
      debitAccountId: form.debit,
      description: form.description,
      periodId: form.periodId,
      createdAt: form.created,
    };
    if (this.isEdit) {
      this.journalService.EditJournal(journal).subscribe({
        next: (result) => {
          this.toastr.success(result.message, 'Edit Journal');
          this.router.navigate(['/Journal', 'List']);
        },
        error: (error) => {
          console.log(error),
            this.toastr.error(error.error.message, 'Edit Journal');
        },
      });
    } else {
      this.journalService.SaveJournal(journal).subscribe({
        next: (result) => {
          this.toastr.success(result.message, 'Create Journal');
          this.router.navigate(['/Journal', 'Create']).then(() => {
            this.journalAmount = 0;
            this.JournalForm.patchValue({
              amount: 0,
              details: '',
              description: '',
              code: this.JournalForm.get('code').value + 1,
              created: this.GetLocaleDateTime(new Date()),
              costCenter: null,
            });
            this.GetCreditBalance();
            this.GetDebitBalance();
          });
        },
        error: (error) => {
          console.log(error),
            this.toastr.error(error.error.message, 'Create Journal');
        },
      });
    }
  }
  DeleteJournal(id) {
    this.journalService.DeleteJournal(id).subscribe({
      next: (result) => this.toastr.success(result.message, 'Delete Journal'),
      error: (err) => this.toastr.error(err.error.message, 'Delete Journal'),
      complete: () => this.router.navigate(['/Journal', 'List']),
    });
  }
  GetLocaleDateTime(date: Date) {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }
}
