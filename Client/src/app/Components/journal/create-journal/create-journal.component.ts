import { Component, HostListener, signal, WritableSignal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { JournalService } from '../../../Services/journal.service';
import { Title } from '@angular/platform-browser';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';

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
import { NgSelectComponent } from '@ng-select/ng-select';
import { BidiModule } from '@angular/cdk/bidi';
import { environment } from '../../../../environments/environment';

@Component({
  imports: [
    RouterLink,
    ReactiveFormsModule,
    NgFor,
    NgClass,
    NgSelectComponent,
    BidiModule,
  ],
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
    private titleService: Title
  ) {
    this.titleService.setTitle('Create Journal');
    var JournalId = this.route.snapshot.params['id'];

    if (JournalId) {
      this.isEdit = true;
      this.titleService.setTitle('Edit Journal');
      this.journalService.GetEditJournal(JournalId).subscribe({
        next: (result) => {
          if (result.isSucceed) {
            this.Journal.set(result.data);
            this.intializeForm(result.data);
          } else {
            Swal.fire('Invalid Journal', result.message, 'info').then((res) => {
              if (res.isConfirmed) {
                this.router.navigate(['/Journal', 'List']);
              }
            });
          }
        },
      });
    } else {
      const periodId = this.route.snapshot.queryParams['periodId'] || 0;
      this.journalService.GetNewJournal(periodId).subscribe({
        next: (result) => {
          if (result.isSucceed) {
            this.Journal.set(result.data);
            this.intializeForm(result.data);
          } else {
            Swal.fire('Invalid Journal', result.message, 'info').then((res) => {
              if (res.isConfirmed) {
                this.router.navigate(['/Journal', 'List']);
              }
            });
          }
        },
      });
    }
  }

  intializeForm(journal: Journal) {
    this.JournalForm = new FormGroup({
      id: new FormControl(journal.id),
      details: new FormControl(journal.detail, Validators.required),
      amount: new FormControl(journal.amount, Validators.min(0.1)),
      debit: new FormControl(journal.debitAccountId, Validators.required),
      credit: new FormControl(journal.creditAccountId, Validators.required),
      costCenter: new FormControl(journal.costCenterId ?? ''),
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
    // this.GetCreditBalance();
    // this.GetDebitBalance();
  }

  GetCreditBalance() {}
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
      error: (error) => {
        Swal.fire({
          title: 'Get Debit Balance',
          text: error ?? 'An Error Happend',
          icon: 'error',
          showConfirmButton: true,
          timer: environment.sweetAlertTimeOut,
        });
      },
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
      costCenterId: form.costCenter == '' ? null : form.costCenter,
      creditAccountId: form.credit,
      debitAccountId: form.debit,
      description: form.description,
      periodId: form.periodId,
      createdAt: form.created,
    };

    if (this.isEdit) {
      this.journalService.EditJournal(journal).subscribe({
        next: (result) => {
          Swal.fire({
            title: 'Edit Journal',
            text: result.message,
            icon: 'success',
            showConfirmButton: false,
            timer: environment.sweetAlertTimeOut,
          });
          //this.router.navigate(['/Journal', 'List']);
        },
        error: (error) => {
          Swal.fire({
            title: 'Edit Journal',
            text: error.error.message ?? 'An Error Happend',
            icon: 'error',
            showConfirmButton: true,
          });
        },
      });
    } else {
      this.journalService.SaveJournal(journal).subscribe({
        next: (result) => {
          Swal.fire({
            title: 'Create Journal',
            text: result.message,
            icon: 'success',
            showConfirmButton: false,
            timer: environment.sweetAlertTimeOut,
          });
          this.router.navigate(['/Journal', 'Create']).then(() => {
            this.journalAmount = 0;
            this.JournalForm.patchValue({
              amount: 0,
              details: '',
              description: '',
              code: this.JournalForm.get('code').value + 1,
              costCenter: '',
              created: this.GetDateTimePlusOneMinute(
                this.JournalForm.get('created').value
              ),
            });
            // this.GetCreditBalance();
            // this.GetDebitBalance();
          });
        },
        error: (error) => {
          Swal.fire({
            title: 'Create Journal',
            text: error.error.message ?? 'An Error Happend',
            icon: 'error',
            showConfirmButton: true,
          });
        },
      });
    }
  }
  DeleteJournal(id: number) {
    Swal.fire({
      title: 'Are you sure?',
      text: 'Are you sure you want to delete this journal?',
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: 'Yes, delete it!',
      cancelButtonText: 'Cancel',
    }).then((result) => {
      if (result.isConfirmed) {
        this.journalService.DeleteJournal(id).subscribe({
          next: (res) => {
            Swal.fire({
              title: 'Delete Journal',
              text: res.message,
              icon: 'success',
              showConfirmButton: false,
            });
          },
          error: (err) => {
            Swal.fire({
              title: 'Delete Journal',
              text: err.error.message,
              icon: 'error',
              showConfirmButton: true,
            });
          },
          complete: () => {
            this.router.navigate(['/Journal', 'List']);
          },
        });
      }
    });
  }

  GetAccountBalance(isDebit) {
    const account = isDebit ? 'debit' : 'credit';

    const accountId = this.JournalForm.get(account)?.value;

    if (!accountId) {
      Swal.fire({
        title: 'Get Balance',
        text: "Please Select Account To Get It's Balance",
        icon: 'error',
        showConfirmButton: true,
        timer: environment.sweetAlertTimeOut,
      });
      return;
    }

    this.accountService.GetAccountBalance(accountId).subscribe({
      next: (result) => {
        const amount = result.balance;
        const accountType = amount <= 0 ? 'Credit' : 'Debit';
        const color = amount <= 0 ? 'red' : 'green';
        const finalAmount =
          amount < 0 ? (amount * -1).toFixed(2) : amount.toFixed(2);
        const msgHtml = `<span style='font-size: 2rem; color: ${color};'>${finalAmount}</span> <br/> <span class='margin-top:0.5rem'>${accountType}</span>`;

        Swal.fire({
          title: 'Balance',
          html: msgHtml,
          icon: 'info',
          showConfirmButton: true,
        });
      },
      error: (error) => {
        Swal.fire({
          title: 'Get Credit Balance',
          text: error ?? 'An Error Happend',
          icon: 'error',
          showConfirmButton: true,
          timer: environment.sweetAlertTimeOut,
        });
      },
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
  GetDateTimePlusOneMinute(dateString: string) {
    const date = new Date(dateString);
    date.setMinutes(date.getMinutes() + 1);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    return `${year}-${month}-${day}T${hours}:${minutes}`;
  }

  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.ctrlKey && event.key === 'Enter') {
      event.preventDefault();
      if (this.JournalForm.valid) {
        this.SaveJournal();
      } else {
        Swal.fire('Invalid Form', 'Please Fill All Required Fields', 'info');
      }
    }
  }
}
