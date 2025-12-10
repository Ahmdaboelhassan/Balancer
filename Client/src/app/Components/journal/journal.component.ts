import { Component, OnInit, signal } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { DateRangeComponent } from '../date-range/date-range.component';
import { JournalListComponent } from './journal-list/journal-list.component';
import { JournalListItem } from '../../Interfaces/Response/JournalListItem';
import { JournalService } from '../../Services/journal.service';
import Swal from 'sweetalert2';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-journal',
  imports: [DateRangeComponent, JournalListComponent, DateRangeComponent],
  templateUrl: './journal.component.html',
  styleUrl: './journal.component.css',
  providers: [CurrencyPipe],
})
export class JournalComponent implements OnInit {
  journals: JournalListItem[] = [];
  constructor(
    private titleService: Title,
    private journalService: JournalService,
    private currency: CurrencyPipe
  ) {
    this.titleService.setTitle('Journals');
  }
  ngOnInit(): void {
    this.journalService.advancedSearch$.subscribe({
      next: (keys) => {
        this.AdvancedSearchJournals(keys);
      },
    });

    this.journalService.journalsSummary$.subscribe({
      next: () => {
        const grouped = this.journals.reduce((acc, item) => {
          const t = item.type;

          if (!acc[t]) {
            acc[t] = {
              count: 0,
              totalAmount: 0,
            };
          }

          acc[t].count++;
          acc[t].totalAmount += item.amount;

          return acc;
        }, {});

        const addAmount = this.currency.transform(
          grouped[1]?.totalAmount || 0,
          'USD',
          'symbol',
          '1.2-2'
        );
        const subtractAmount = this.currency.transform(
          grouped[2]?.totalAmount * -1 || 0,
          'USD',
          'symbol',
          '1.2-2'
        );
        const dueAmount = this.currency.transform(
          grouped[3]?.totalAmount || 0,
          'USD',
          'symbol',
          '1.2-2'
        );
        const forwardAmount = this.currency.transform(
          grouped[4]?.totalAmount || 0,
          'USD',
          'symbol',
          '1.2-2'
        );

        Swal.fire({
          title: 'Journals Summery',
          icon: 'info',
          width: '600px',
          html: `
            <div class="overflow-x-auto">
              <table class="min-w-full border border-gray-300 rounded-lg">
                <thead class="bg-gray-100 border-b text-center">
                  <tr>
                    <th class="px-4 py-2 text-left font-semibold border-r">Journal Type</th>
                    <th class="px-4 py-2 text-left font-semibold border-r">Count</th>
                    <th class="px-4 py-2 text-left font-semibold">Total Amount</th>
                  </tr>
                </thead>

                <tbody>
                  <tr class="border-b text-end">
                    <td class="py-2 px-4 border-r font-semibold text-start bg-gray-100 border-b">Add</td>
                    <td class="px-4 py-2 border-r text-green-500">${
                      grouped[1]?.count || 0
                    }</td>
                    <td class="px-4 py-2 text-green-500">${addAmount}</td>
                  </tr>
                  <tr class="border-b bg-gray-50 text-end">
                    <td class="px-4 py-2 border-r font-semibold text-start bg-gray-100 border-b">Subtract</td>
                    <td class="px-4 py-2 border-r text-red-500">${
                      grouped[2]?.count || 0
                    }</td>
                    <td class="px-4 py-2 text-red-500">${subtractAmount}</td>
                  </tr>
                  <tr class="border-b text-end">
                    <td class="px-4 py-2 border-r font-semibold text-start bg-gray-100 border-b">Forward</td>
                    <td class="px-4 py-2 border-r text-orange-500">${
                      grouped[4]?.count || 0
                    }</td>
                    <td class="px-4 py-2 text-orange-500">${forwardAmount}</td>
                  </tr>
                  <tr class="border-b bg-gray-50 text-end">
                    <td class="px-4 py-2 border-r font-semibold text-start bg-gray-100 border-b">Due</td>
                    <td class="px-4 py-2 border-r text-blue-500">${
                      grouped[3]?.count || 0
                    }</td>
                    <td class="px-4 py-2 text-blue-500">${dueAmount}</td>
                  </tr>
                
                </tbody>
              </table>
            </div>
          `,
          showConfirmButton: true,
        });
      },
    });
  }

  GetJournals(dates: any) {
    if (!dates.from || !dates.to) {
      return;
    }

    this.journalService
      .GetJournals(dates.from, dates.to)
      .subscribe((result) => {
        this.journals = result;
      });
  }

  AdvancedSearchJournals(keys: any) {
    this.journalService.AvancedSearchJournals(keys).subscribe({
      next: (result) => {
        this.journals = result;
      },
    });
  }
}
