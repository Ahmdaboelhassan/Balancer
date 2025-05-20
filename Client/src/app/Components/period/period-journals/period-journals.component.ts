import { Component, OnInit } from '@angular/core';
import { JournalListComponent } from '../../journal/journal-list/journal-list.component';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { JournalService } from '../../../Services/journal.service';
import { Periodjournals } from '../../../Interfaces/Response/Periodjournals';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-period-journals',
  imports: [JournalListComponent, RouterLink, NgClass],
  templateUrl: './period-journals.component.html',
  styleUrl: './period-journals.component.css',
})
export class PeriodJournalsComponent implements OnInit {
  model: Periodjournals | any;
  isCurrentPeriod = false;
  periodId = 0;

  constructor(
    private route: ActivatedRoute,
    private journalService: JournalService
  ) {}

  ngOnInit() {
    this.route.params.subscribe((params) => {
      const periodId = params['id'];
      this.getPeriodJournals(periodId);
      this.periodId = periodId;
      this.isCurrentPeriod = periodId == 0;
    });
  }

  getPeriodJournals(periodId) {
    this.journalService.GetPeriodJournals(periodId).subscribe((result) => {
      this.model = result;
    });
  }
}
