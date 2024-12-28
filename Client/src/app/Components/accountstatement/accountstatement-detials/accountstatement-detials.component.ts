import { Component, OnInit } from '@angular/core';
import { AccountStatement } from '../../../Interfaces/Response/AccountStatement';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReportService } from '../../../Services/report.service';
import { ToastrService } from 'ngx-toastr';
import { NgClass } from '@angular/common';

@Component({
  selector: 'app-accountstatement-detials',
  imports: [RouterLink, NgClass],
  templateUrl: './accountstatement-detials.component.html',
  styleUrl: './accountstatement-detials.component.css',
})
export class AccountstatementDetialsComponent implements OnInit {
  accountStatement: AccountStatement;

  constructor(
    private reportService: ReportService,
    private route: ActivatedRoute,
    private toestr: ToastrService
  ) {}

  ngOnInit(): void {
    const from = this.route.snapshot.queryParams['from'];
    const to = this.route.snapshot.queryParams['to'];
    const account = this.route.snapshot.queryParams['account'];
    const costcenter = this.route.snapshot.queryParams['costCenter'];
    const openingbalance = this.route.snapshot.queryParams['openingBalance'];

    if (!account) {
      this.toestr.error('Please Select Account');
    }

    this.reportService
      .GetAccountStatemnt(from, to, account, costcenter, openingbalance)
      .subscribe({
        next: (accountStatement) => (this.accountStatement = accountStatement),
      });
  }
}
