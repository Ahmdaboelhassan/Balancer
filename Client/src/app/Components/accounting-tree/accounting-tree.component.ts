import { Component, OnDestroy, OnInit, ViewEncapsulation } from '@angular/core';
import { AccountTreeItem } from '../../Interfaces/Response/AccountingTreeItem';
import { AccountService } from '../../Services/account.service';
import { Title } from '@angular/platform-browser';
import { LoadingService } from '../../Services/loading.service';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-accounting-tree',
  encapsulation: ViewEncapsulation.None,
  templateUrl: './accounting-tree.component.html',
  styleUrl: './accounting-tree.component.css',
})
export class AccountingTreeComponent implements OnInit {
  primaryAccounts: AccountTreeItem[] = [];
  marginLeft = 11;

  constructor(
    private accountingService: AccountService,
    private loadingService: LoadingService,
    private titleService: Title,
  ) {
    this.titleService.setTitle('Accounting Tree');
  }

  ngOnInit(): void {
    this.accountingService.GetPrimaryAccounts().subscribe({
      next: (accounts) => (this.primaryAccounts = accounts),
    });
  }
  GetAccountChild(id, e) {
    this.loadingService.DisableLoading();
    const targetElement = e.target as HTMLDetailsElement;
    if (!targetElement.open) {
      targetElement.querySelectorAll('.child').forEach((el) => el.remove());
      return;
    }

    const targetElementloader =
      targetElement.querySelector('.AccountTreeloader');

    targetElementloader.className = 'AccountTreeloader inline-block';

    this.accountingService.GetChilds(id).subscribe({
      next: (accounts) => {
        accounts.forEach((el) => {
          const summary = document.createElement('summary');
          const loader = '<div class="AccountTreeloader hidden"></div>';
          const editLink = `<a href="/#/Account/Edit/${el.id}" target="_blank" style="color:#0d6efd; cursor:pointer;" class="hover:opacity-100 opacity-0 transition-all hover:scale-110">
              <i class="fa-solid fa-pen-to-square fa-xs"></i></a>`;

          summary.innerHTML = `${loader} ${el.name} | ${el.number} ${editLink}`;
          if (el.isArchived) summary.style.textDecoration = 'line-through';

          let child = document.createElement('details');
          child.className = 'my-3 cursor-pointer text-xl child font-medium';
          child.style.marginLeft = this.marginLeft * el.level + 'px';

          child.addEventListener('toggle', (e) => {
            e.stopPropagation();
            this.GetAccountChild(el.id, e);
          });

          child.append(summary);
          targetElement.appendChild(child);
        });
        targetElementloader.className = 'AccountTreeloader hidden';
      },
    });
    this.loadingService.EnableLoading();
  }
}
