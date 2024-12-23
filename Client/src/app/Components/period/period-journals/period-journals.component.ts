import { Component, OnInit } from '@angular/core';
import { JournalListComponent } from '../../journal/journal-list/journal-list.component';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { JournalService } from '../../../Services/journal.service';
import { Periodjournals } from '../../../Interfaces/Response/Periodjournals';

@Component({
  selector: 'app-period-journals',
  imports: [JournalListComponent, RouterLink],
  templateUrl: './period-journals.component.html',
  styleUrl: './period-journals.component.css',
})
export class PeriodJournalsComponent implements OnInit {
  periodId: number = 0;
  model: Periodjournals | any;

  constructor(
    private route: ActivatedRoute,
    private journalService: JournalService
  ) {}

  ngOnInit() {
    this.route.params.subscribe((params) => {
      this.periodId = params['id'];
      this.getPeriodJournals();
    });
  }

  getPeriodJournals() {
    this.journalService.GetPeriodJournals(this.periodId).subscribe((result) => {
      console.log(result);
      this.model = result;
    });
  }
}
