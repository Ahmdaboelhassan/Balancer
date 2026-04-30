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
import { AccountsBalance } from '../../../Interfaces/Response/AccountsBalance';

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
  iconMap: Record<number, string> = {
    1: 'fa-solid fa-square-plus text-green-500',
    2: 'fa-solid fa-square-minus text-red-400',
    3: 'fa-solid fa-thumbtack text-blue-400',
    4: 'fa-solid fa-reply fa-flip-horizontal text-orange-400',
    5: 'fa-solid fa-wallet text-purple-400',
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private journalService: JournalService,
    private accountService: AccountService,
    private titleService: Title,
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

  GetAdjacentJournal(isNext) {
    const journalId = this.JournalForm.get('created').value;
    this.journalService.GetAdjacentJournal(journalId, isNext).subscribe({
      next: (result) => {
        if (result.isSucceed) {
          this.Journal.set(result.data);
          this.intializeForm(result.data);
          this.router.navigate(['/Journal', 'Edit', result.data.id]);
        } else {
          Swal.fire('Invalid Action', result.message, 'info');
        }
      },
    });
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
        this.GetLocaleDateTime(new Date(journal.createdAt)),
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
          this.router.navigate(['/Journal', 'Edit', result.data]);
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
                this.JournalForm.get('created').value,
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

  AdjustBudget() {
    const amount = this.JournalForm.get('amount')?.value;

    if (!amount || amount <= 0) {
      Swal.fire({
        title: 'Invalid Amount',
        text: 'Please enter a valid amount',
        icon: 'warning',
      });
      return;
    }

    // Step 1: Are you sure?
    Swal.fire({
      title: 'Are you sure?',
      text: `You are about to adjust the budget by ${amount}`,
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: 'Yes, continue',
      cancelButtonText: 'Cancel',
    }).then((confirmResult) => {
      if (!confirmResult.isConfirmed) return;

      // Step 2: Increase or Decrease?
      Swal.fire({
        title: 'Adjustment Type',
        text: 'Do you want to Increase Budget or Increase Saving Target?',
        icon: 'question',
        showDenyButton: true,
        showCancelButton: true,
        confirmButtonText: 'Increase Budget',
        denyButtonText: 'Increase Saving',
      }).then((typeResult) => {
        if (typeResult.isConfirmed || typeResult.isDenied) {
          const request = {
            amount: amount,
            isBudgetIncrease: typeResult.isConfirmed, // true = increase budget , false = increase savings target
          };

          // Step 3: Send request
          this.journalService.AdjustBudget(request).subscribe({
            next: (result) => {
              Swal.fire({
                title: 'Adjust Budget',
                text: result.message,
                icon: 'success',
                showConfirmButton: false,
                timer: environment.sweetAlertTimeOut,
              });
            },
            error: (error) => {
              Swal.fire({
                title: 'Adjust Budget',
                text: error.error?.message ?? 'An Error Happened',
                icon: 'error',
                showConfirmButton: true,
              });
            },
          });
        }
      });
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
    const currentDate = new Date();
    let startMonth = currentDate.getMonth();
    let endMonth = currentDate.getMonth() + 1;

    const from = new Date(currentDate.getFullYear(), startMonth, 2)
      .toISOString()
      .split('T')[0];

    const to = new Date(currentDate.getFullYear(), endMonth, 1)
      .toISOString()
      .split('T')[0];

    this.accountService
      .GetBalanceBasedOnType(accountId, from, to, '')
      .subscribe({
        next: (result) => {
          const msgHtml = this.GetAccountBalanceHTMLTemplate(result, from, to);
          Swal.fire({
            title: 'Balance',
            html: msgHtml,
            icon: undefined,
            showConfirmButton: true,
          });
        },
        error: (error) => {
          Swal.fire({
            title: 'Get Balance',
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

  GetAccountBalanceHTMLTemplate(result: AccountsBalance, from, to) {
    const isCredit = result.isCredit;
    const balance = result.balance.replace(/^\(|\)$/g, '');
    const accountType = isCredit ? 'Credit' : 'Debit';
    const params = new URLSearchParams({
      openingBalance: (!result.isRevExp).toString(),
      account: result.accountId.toString(),
      from: from,
      to: to,
    });

    const url = `/#/AccountStatement/Get?${params.toString()}`;

    const badgeColor = isCredit
      ? 'background:#fff0f0; color:#c0392b; border:1px solid #f5c6cb;'
      : 'background:#f0fff4; color:#1a7f4b; border:1px solid #b7dfc7;';

    const amountColor = isCredit ? '#c0392b' : '#1a7f4b';
    const arrowIcon = isCredit ? '↓' : '↑';

    const dateSection = result.isRevExp
      ? `
        <div style="
          display: flex;
          justify-content: center;
          gap: 1.5rem;
          margin: 1rem 0 1.5rem;
          flex-wrap: wrap;
        ">
          <div style="
            background: #f8f9fc;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            padding: 8px 16px;
            font-size: 0.78rem;
            color: #64748b;
            font-family: 'Segoe UI', sans-serif;
            letter-spacing: 0.03em;
          ">
            <span style="color:#94a3b8; text-transform:uppercase; font-size:0.68rem; display:block; margin-bottom:2px;">From</span>
            <b style="color:#334155;">${from}</b>
          </div>
          <div style="
            background: #f8f9fc;
            border: 1px solid #e2e8f0;
            border-radius: 10px;
            padding: 8px 16px;
            font-size: 0.78rem;
            color: #64748b;
            font-family: 'Segoe UI', sans-serif;
            letter-spacing: 0.03em;
          ">
            <span style="color:#94a3b8; text-transform:uppercase; font-size:0.68rem; display:block; margin-bottom:2px;">To</span>
            <b style="color:#334155;">${to}</b>
          </div>
        </div>
      `
      : '<div style="margin-bottom:1.5rem;"></div>';

    const msgHtml = `
          <div style="
            font-family: 'Segoe UI', system-ui, sans-serif;
            padding: 0.5rem 0.25rem 0.75rem;
            text-align: center;
          ">

            <!-- Account Badge -->
            <div style="margin-bottom: 1.25rem;">
              <span style="
                display: inline-flex;
                align-items: center;
                gap: 6px;
                padding: 5px 14px;
                border-radius: 999px;
                font-size: 0.75rem;
                font-weight: 600;
                letter-spacing: 0.06em;
                text-transform: uppercase;
                ${badgeColor}
              ">
                <span style="font-size:1rem;">${arrowIcon}</span>
                ${accountType} Account
              </span>
            </div>

        <!-- Divider -->
          <div style="
            width: 40px;
            height: 3px;
            background: linear-gradient(90deg, transparent, ${amountColor}, transparent);
            margin: 0 auto 1.25rem;
            border-radius: 2px;
          "></div>

          <!-- Balance Amount -->
          <div style="
            font-size: 2.75rem;
            font-weight: 700;
            color: ${amountColor};
            letter-spacing: -0.03em;
            line-height: 1;
            margin-bottom: 0.4rem;
          ">
            ${balance}
          </div>

          <div style="
            font-size: 0.75rem;
            text-transform: uppercase;
            letter-spacing: 0.1em;
            color: #94a3b8;
            margin-bottom: 1.5rem;
          ">
            Current Balance
          </div>

          <!-- Date Pills -->
          ${dateSection}

          <!-- CTA Button -->
          <a href="${url}" target="_blank" style="
            display: inline-flex;
            align-items: center;
            gap: 8px;
            padding: 11px 22px;
            background: linear-gradient(135deg, #1e40af 0%, #2563eb 100%);
            color: #fff;
            border-radius: 10px;
            text-decoration: none;
            font-size: 0.85rem;
            font-weight: 600;
            letter-spacing: 0.02em;
            box-shadow: 0 4px 14px rgba(37,99,235,0.35);
            transition: all 0.2s ease;
            "
            onmouseover="this.style.boxShadow='0 6px 20px rgba(37,99,235,0.5)'; this.style.transform='translateY(-1px)';"
            onmouseout="this.style.boxShadow='0 4px 14px rgba(37,99,235,0.35)'; this.style.transform='translateY(0)';"
            >
              <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round">
                <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z"/>
                <polyline points="14 2 14 8 20 8"/>
                <line x1="16" y1="13" x2="8" y2="13"/>
                <line x1="16" y1="17" x2="8" y2="17"/>
              </svg>
              View Account Statement
            </a>

          </div>
        `;

    return msgHtml;
  }

  get iconClass(): string {
    if (this.Journal()) {
      return this.iconMap[this.Journal().type] ?? '';
    }
    return '';
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
