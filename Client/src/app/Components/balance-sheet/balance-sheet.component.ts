import { Component } from '@angular/core';
import { AccountSummary } from '../../Interfaces/Response/AccountSummary';
import { ReportService } from '../../Services/report.service';
import { Title } from '@angular/platform-browser';
import { DateRangeComponent } from '../date-range/date-range.component';
import { CurrencyPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RatesService, TransfersRates } from '../../Services/rates.service';
import { catchError, of } from 'rxjs';
import Swal from 'sweetalert2';
import { LoadingService } from '../../Services/loading.service';

@Component({
  selector: 'app-balance-sheet',
  imports: [DateRangeComponent, CurrencyPipe, RouterLink],
  templateUrl: './balance-sheet.component.html',
  styleUrl: './balance-sheet.component.css',
})
export class BalanceSheetComponent {
  accounts: AccountSummary[] = [];
  rates: TransfersRates;

  constructor(
    private reportService: ReportService,
    private ratesService : RatesService,
    private loadingService : LoadingService,
    private titleServive: Title,
  ) {
    this.titleServive.setTitle('Balance Sheet');

    this.ratesService.getRates().subscribe({
      next: (rates) => { this.rates = rates; }
    });
  }

  from;
  to;

  GetBalanceSheet(dates: any) {
    this.from = dates.from;
    this.to = dates.to;
    
    this.loadingService.LoadingStarted();
    this.reportService.GetBalanceSheet(dates.to, dates.maxLevel)
      .subscribe({
        next: (accounts) => { this.accounts = accounts; }
      });
  }

  ShowRates(amount: number) {
    if (!this.rates) {
      Swal.fire('', 'Rates are not available', 'info');
      return;
    }

    const rows = [
      { label: 'EGP', rate: 1, value: amount },
      { label: 'USD', rate: this.rates.usdToEgp, value: amount / this.rates.usdToEgp },
      { label: 'Gold 24K (gram)', rate: this.rates.gold24EgpPerGram, value: amount / this.rates.gold24EgpPerGram },
      { label: 'Gold 22K (gram)', rate: this.rates.gold22EgpPerGram, value: amount / this.rates.gold22EgpPerGram },
      { label: 'Gold 20K (gram)', rate: this.rates.gold20EgpPerGram, value: amount / this.rates.gold20EgpPerGram },
      { label: 'Gold 18K (gram)', rate: this.rates.gold18EgpPerGram, value: amount / this.rates.gold18EgpPerGram },
      { label: 'Silver (gram)', rate: this.rates.silverEgpPerGram, value: amount / this.rates.silverEgpPerGram },
      { label: 'BTC', rate: this.rates.btcEgp, value: amount / this.rates.btcEgp },
    ];

    Swal.fire({
      title: '',
      icon: 'info',
      width: '650px',
      html: `
        <div class="overflow-x-auto">
          <table class="min-w-full border border-gray-300 rounded-lg text-sm lg:text-base">
            <tbody>
              ${rows
                .map(
                  (row) => `
                  <tr class="border-b text-end">
                    <td class="py-2 px-4 border-r font-semibold text-start bg-gray-100">${row.label}</td>
                    <td class="px-2 py-2 border-r text-start">${row.rate.toLocaleString('en-US', { maximumFractionDigits: 2 })}</td>
                    <td class="px-2 py-2 font-semibold text-start">${row.value.toLocaleString('en-US', { maximumFractionDigits: 6 })}</td>
                  </tr>`
                )
                .join('')}
            </tbody>
          </table>
        </div>
      `,
    });
  }
}