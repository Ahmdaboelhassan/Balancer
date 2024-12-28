import { Routes } from '@angular/router';
import { HomeComponent } from './Components/home/home.component';
import { AuthComponent } from './Components/auth/auth.component';
import { CreatePeriodComponent } from './Components/period/create-period/create-period.component';
import { SearchPeriodComponent } from './Components/period/search-period/search-period.component';
import { PeriodComponent } from './Components/period/period.component';
import { CreateJournalComponent } from './Components/journal/create-journal/create-journal.component';
import { SearchJournalComponent } from './Components/journal/search-journal/search-journal.component';
import { JournalComponent } from './Components/journal/journal.component';
import { PeriodJournalsComponent } from './Components/period/period-journals/period-journals.component';
import { AccountComponent } from './Components/account/account.component';
import { CreateAccountComponent } from './Components/account/create-account/create-account.component';
import { CreateCostcenterComponent } from './Components/costcenter/create-costcenter/create-costcenter.component';
import { CostcenterComponent } from './Components/costcenter/costcenter.component';
import { AccountstatementComponent } from './Components/accountstatement/accountstatement.component';
import { AccountstatementDetialsComponent } from './Components/accountstatement/accountstatement-detials/accountstatement-detials.component';

export const routes: Routes = [
  { path: 'Home', loadComponent: () => HomeComponent },
  { path: 'Auth', loadComponent: () => AuthComponent },
  {
    path: 'Journal',
    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreateJournalComponent },
      { path: 'Create/:periodId', loadComponent: () => CreateJournalComponent },
      { path: 'Edit/:id', loadComponent: () => CreateJournalComponent },
      { path: 'Search', loadComponent: () => SearchJournalComponent },
      { path: 'List', loadComponent: () => JournalComponent },
    ],
  },
  {
    path: 'Period',
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
    path: 'Account',
    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreateAccountComponent },
      { path: 'Edit/:id', loadComponent: () => CreateAccountComponent },
      { path: 'List', loadComponent: () => AccountComponent },
    ],
  },
  {
    path: 'CostCenter',
    children: [
      { path: '', redirectTo: 'List', pathMatch: 'full' },
      { path: 'Create', loadComponent: () => CreateCostcenterComponent },
      { path: 'Edit/:id', loadComponent: () => CreateCostcenterComponent },
      { path: 'List', loadComponent: () => CostcenterComponent },
    ],
  },
  {
    path: 'AccountStatement',
    children: [
      { path: '', loadComponent: () => AccountstatementComponent },
      { path: 'Get', loadComponent: () => AccountstatementDetialsComponent },
    ],
  },
  { path: '**', loadComponent: () => HomeComponent, pathMatch: 'full' },
];
