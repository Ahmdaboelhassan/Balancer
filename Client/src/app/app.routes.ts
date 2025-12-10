import { Routes } from '@angular/router';
import { HomeComponent } from './Components/home/home.component';
import { AuthComponent } from './Components/auth/auth.component';
import { CreatePeriodComponent } from './Components/period/create-period/create-period.component';
import { SearchPeriodComponent } from './Components/period/search-period/search-period.component';
import { PeriodComponent } from './Components/period/period.component';
import { CreateJournalComponent } from './Components/journal/create-journal/create-journal.component';
import { JournalComponent } from './Components/journal/journal.component';
import { PeriodJournalsComponent } from './Components/period/period-journals/period-journals.component';
import { AccountComponent } from './Components/account/account.component';
import { CreateAccountComponent } from './Components/account/create-account/create-account.component';
import { CreateCostcenterComponent } from './Components/costcenter/create-costcenter/create-costcenter.component';
import { CostcenterComponent } from './Components/costcenter/costcenter.component';
import { AccountstatementComponent } from './Components/accountstatement/accountstatement.component';
import { AccountstatementDetialsComponent } from './Components/accountstatement/accountstatement-detials/accountstatement-detials.component';
import { IncomeStatementComponent } from './Components/income-statement/income-statement.component';
import { AccountingTreeComponent } from './Components/accounting-tree/accounting-tree.component';
import { AccountsSummaryComponent } from './Components/accounts-summary/accounts-summary.component';
import { AccountsOverviewComponent } from './Components/accounts-overview/accounts-overview.component';
import { authGuard } from './Guards/auth.guard';
import { AccountComparerComponent } from './Components/account-comparer/account-comparer.component';
import { AccountComparerDetailsComponent } from './Components/account-comparer/account-comparer-details/account-comparer-details.component';
import { SettingsComponent } from './Components/settings/settings.component';
import { DashboardSettingsComponent } from './Components/dashboard-settings/dashboard-settings.component';
import { AccountBudgetSettingsComponent } from './Components/dashboard-settings/account-budget-settings/account-budget-settings.component';
import { EvaluationComponent } from './Components/evaluation/evaluation.component';
import { CreateEvaluationComponent } from './Components/evaluation/create-evaluation/create-evaluation.component';
import { BalanceSheetComponent } from './Components/balance-sheet/balance-sheet.component';

export const routes: Routes = [
  {
    path: 'Home',
    canActivate: [authGuard],
    loadComponent: () => HomeComponent,
  },
  {
    path: 'Auth',
    loadComponent: () => AuthComponent,
  },
  {
    path: 'Journal',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreateJournalComponent },
      { path: 'Create/:periodId', loadComponent: () => CreateJournalComponent },
      { path: 'Edit/:id', loadComponent: () => CreateJournalComponent },
      { path: 'List', loadComponent: () => JournalComponent },
    ],
  },
  {
    path: 'Period',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreatePeriodComponent },
      { path: 'Edit/:id', loadComponent: () => CreatePeriodComponent },
      { path: 'Search', loadComponent: () => SearchPeriodComponent },
      { path: 'List', loadComponent: () => PeriodComponent },
      {
        path: 'PeriodJournals/:id',
        loadComponent: () => PeriodJournalsComponent,
      },
    ],
  },
  {
    path: 'Evaluation',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreateEvaluationComponent },
      { path: 'Edit/:id', loadComponent: () => CreateEvaluationComponent },
      { path: 'List', loadComponent: () => EvaluationComponent },
      {
        path: 'PeriodJournals/:id',
        loadComponent: () => PeriodJournalsComponent,
      },
    ],
  },
  {
    path: 'Account',
    canActivate: [authGuard],

    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreateAccountComponent },
      { path: 'Edit/:id', loadComponent: () => CreateAccountComponent },
      { path: 'List', loadComponent: () => AccountComponent },
    ],
  },
  {
    path: 'CostCenter',
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreateCostcenterComponent },
      { path: 'Edit/:id', loadComponent: () => CreateCostcenterComponent },
      { path: 'List', loadComponent: () => CostcenterComponent },
    ],
  },
  {
    path: 'AccountStatement',
    canActivate: [authGuard],
    children: [
      { path: '', loadComponent: () => AccountstatementComponent },
      { path: 'Get', loadComponent: () => AccountstatementDetialsComponent },
    ],
  },
  {
    path: 'IncomeStatement',
    canActivate: [authGuard],
    loadComponent: () => IncomeStatementComponent,
  },
  {
    path: 'AccountingTree',
    canActivate: [authGuard],
    loadComponent: () => AccountingTreeComponent,
  },
  {
    path: 'AccountsSummary',
    canActivate: [authGuard],
    loadComponent: () => AccountsSummaryComponent,
  },
  {
    path: 'AccountsOverview',
    canActivate: [authGuard],
    loadComponent: () => AccountsOverviewComponent,
  },
  {
    path: 'BalanceSheet',
    canActivate: [authGuard],
    loadComponent: () => BalanceSheetComponent,
  },
  {
    path: 'AccountComparer',
    canActivate: [authGuard],
    children: [
      { path: '', loadComponent: () => AccountComparerComponent },
      { path: 'Get', loadComponent: () => AccountComparerDetailsComponent },
    ],
  },
  {
    path: 'Settings',
    canActivate: [authGuard],
    loadComponent: () => SettingsComponent,
  },
  {
    path: 'DashboardSettings',
    canActivate: [authGuard],
    loadComponent: () => DashboardSettingsComponent,
  },
  {
    path: 'BudgetAccountSettings',
    canActivate: [authGuard],
    loadComponent: () => AccountBudgetSettingsComponent,
  },

  { path: '**', loadComponent: () => HomeComponent, pathMatch: 'full' },
];
