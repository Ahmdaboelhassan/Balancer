import {
  Component,
  HostListener,
  Signal,
  signal,
  ViewChild,
} from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { AccountService } from '../../Services/account.service';
import { CostcenterService } from '../../Services/costcenter.service';
import { AccountSelectList } from '../../Interfaces/Response/AccountSelectList';
import { CostCenterSelectList } from '../../Interfaces/Response/CostCenterSelectList';
import { Title } from '@angular/platform-browser';
import { NgSelectComponent } from '@ng-select/ng-select';
import Swal from 'sweetalert2';
import { AccountsBalance } from '../../Interfaces/Response/AccountsBalance';
import { environment } from '../../../environments/environment';

@Component({
  imports: [FormsModule, NgSelectComponent],
  templateUrl: './accountstatement.component.html',
  styleUrl: './accountstatement.component.css',
})
export class AccountstatementComponent {
  from = signal('');
  to = signal('');
  accounts: AccountSelectList[];
  costCenters: CostCenterSelectList[];
  openingBalance: boolean = false;
  ignoreTo: boolean = false;
  ignoreFrom: boolean = false;
  @ViewChild('myForm') myForm!: NgForm;
  iconMap: Record<number, string> = {
        1: 'fa-solid fa-square-plus text-green-500',
        2: 'fa-solid fa-square-minus text-red-400',
        3: 'fa-solid fa-thumbtack text-blue-400',
        4: 'fa-solid fa-reply fa-flip-horizontal text-orange-400',
        5: 'fa-solid fa-wallet text-purple-400',
  };

  constructor(
    private accountService: AccountService,
    private costCenterService: CostcenterService,
    private titleServive: Title,
   

  ) {
    this.GetDefaultDate();
    this.accountService.GetAllAccountSelectList().subscribe({
      next: (accounts) => (this.accounts = accounts),
    });
    this.costCenterService
      .GetAllCostCenterSelectList()
      .subscribe({ next: (costCenters) => (this.costCenters = costCenters) });
    this.titleServive.setTitle('Account Statement');
  }

  GetAccountStatement(form: NgForm) {
    const baseUrl = window.location.origin;
    if (!form) Swal.fire('', 'Invalid Form', 'info');

    const formValue = form.value;
    if (!formValue.account && !formValue.costCenter) {
      Swal.fire('', 'Please Select Account Or Cost Center', 'info');
    } else {
      const params = {
        openingBalance: formValue.openingBalance,
      };

      if (formValue.account) {
        params['account'] = formValue.account;
      }

      if (formValue.costCenter) {
        params['costCenter'] = formValue.costCenter;
      }

      if (formValue.from && !formValue.ignoreFrom) {
        params['from'] = formValue.from;
      }
      if (formValue.to && !formValue.ignoreTo) {
        params['to'] = formValue.to;
      }
      const queryParams = new URLSearchParams(params).toString();
      const hashRoute = `#/AccountStatement/Get?${queryParams}`;

      const fullUrl = `${baseUrl}/${hashRoute}`;
      window.open(fullUrl, '_blank');
    }
  }
  DecrementMonth() {
    let f = new Date(this.from());
    f.setMonth(f.getMonth() - 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }
  IncrementMonth() {
    let f = new Date(this.from());
    f.setMonth(f.getMonth() + 1);

    const firstDay = new Date(f.getFullYear(), f.getMonth(), 2);
    const lastDay = new Date(f.getFullYear(), f.getMonth() + 1, 1);

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }

  GetCurrentDay() {
    const currentDate = new Date();
    this.from.set(this.formatLocalDate(currentDate));
    this.to.set(this.formatLocalDate(currentDate));
  }

  GetDefaultWeekDates() {
    const currentDate = new Date();
    const day = currentDate.getDay();

    const diffToSaturday = day === 6 ? 0 : day + 1;

    const firstDayOfWeek = new Date(currentDate);
    firstDayOfWeek.setDate(currentDate.getDate() - diffToSaturday);

    const lastDayOfWeek = new Date(firstDayOfWeek);
    lastDayOfWeek.setDate(firstDayOfWeek.getDate() + 6);

    this.from.set(this.formatLocalDate(firstDayOfWeek));
    this.to.set(this.formatLocalDate(lastDayOfWeek));
  }

  private formatLocalDate(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
  GetDefaultDate() {
    const currentDate = new Date();
    const firstDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth(),
      2,
    );
    const lastDay = new Date(
      currentDate.getFullYear(),
      currentDate.getMonth() + 1,
      1,
    );

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }

  GetDefaultYear() {
    const currentDate = new Date();

    const firstDay = new Date(currentDate.getFullYear(), 0, 2); // Jan 1
    const lastDay = new Date(currentDate.getFullYear(), 12, 1); // Dec 31

    this.from.set(firstDay.toISOString().split('T')[0]);
    this.to.set(lastDay.toISOString().split('T')[0]);
  }

  GetAccountBalance(form: NgForm) {
    const formValue = form.value;
    const accountId = formValue.account;

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
    if (formValue.ignoreFrom || formValue.ignoreTo) {
      Swal.fire({
        title: 'Get Balance',
        text: 'You Can Not Ignore Dates In Balance Only Mode ',
        icon: 'error',
        showConfirmButton: true,
        timer: environment.sweetAlertTimeOut,
      });
      return;
    }

    this.accountService
      .GetBalanceBasedOnType(
        accountId,
        formValue.from,
        formValue.to,
        formValue.costCenter,
      )
      .subscribe({
        next: (result) => {
          const msgHtml = this.GetAccountBalanceHTMLTemplate(
            result,
            formValue.from,
            formValue.to,
          );
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

     const lastJournal = result.lastJournal;
    const lastJournalSection = lastJournal
      ? `
        <a href="/#/Journal/Edit/${lastJournal.journalId}" target="_blank" style="
          display: block;
          background: #f8f9fc;
          border: 1px solid #e2e8f0;
          border-radius: 12px;
          padding: 14px 16px;
          margin: 1.5rem;
          text-align: left;
          text-decoration: none;
          font-family: 'Segoe UI', sans-serif;
          transition: all 0.2s ease;
          "
          onmouseover="this.style.borderColor='#2563eb'; this.style.boxShadow='0 4px 14px rgba(37,99,235,0.15)';"
          onmouseout="this.style.borderColor='#e2e8f0'; this.style.boxShadow='none';"
        >
          <div style="
            font-size: 0.68rem;
            text-transform: uppercase;
            letter-spacing: 0.1em;
            color: #94a3b8;
            margin-bottom: 8px;
          ">
            Last Journal
          </div>
          <div style="display:flex; justify-content:space-between; gap:12px; margin-bottom:4px;">
            <span style="color:#334155; font-weight:600; font-size:0.85rem; display:inline-flex; align-items:center; gap:6px;">
              ${lastJournal.name ?? '-'}
            </span>
            <span style="color:#334155; font-weight:700; font-size:1.5rem; white-space:nowrap;">
             ${ lastJournal.amount != null ? lastJournal.amount : '' }
             <i class="${this.iconMap[lastJournal.type] ?? ''}"></i>
            </span>
          </div>
          <div style="font-size:0.75rem; color:#64748b; margin-bottom:2px;">
           ${lastJournal.creditAccount ?? '-'} -> 
            ${lastJournal.debitAccount ?? '-'}
            &nbsp;&nbsp;
          </div>
          <div style="font-size:0.72rem; color:#94a3b8;">
            ${new Date(lastJournal.journalDate).toLocaleString()}
          </div>
          ${
            lastJournal.costCenters?.length
              ? `<div style="display:flex; flex-wrap:wrap; gap:4px; margin-top:8px;">
                  ${lastJournal.costCenters
                    .map(
                      (costCenter) => `<span style="
                        display:inline-flex;
                        align-items:center;
                        border-radius:12px;
                        background:#cbd5e1;
                        color:#334155;
                        padding:2px 10px;
                        font-size:0.7rem;
                        font-weight:500;
                      ">${costCenter}</span>`,
                    )
                    .join('')}
                </div>`
              : ''
          }
        </a>
      `
      : '';

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

            <!-- Last Journal -->
            ${lastJournalSection}
          </div>
        `;

    return msgHtml;
  }
  @HostListener('document:keydown', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.ctrlKey && event.key === 'Enter') {
      event.preventDefault();
      this.GetAccountStatement(this.myForm);
    }
  }
}
