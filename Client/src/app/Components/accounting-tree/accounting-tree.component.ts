import { Component, OnInit } from '@angular/core';
import { AccountTreeItem } from '../../Interfaces/Response/AccountingTreeItem';
import { AccountService } from '../../Services/account.service';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-accounting-tree',
  imports: [],
  templateUrl: './accounting-tree.component.html',
  styleUrl: './accounting-tree.component.css',
})
export class AccountingTreeComponent implements OnInit {
  //closest
  Click() {
    //closest
    console.log('ahmed');
  }
  primaryAccounts: AccountTreeItem[] = [];
  marginLeft = 10;

  constructor(
    private accountingService: AccountService,
    private titleService: Title
  ) {
    this.titleService.setTitle('Accounting Tree');
  }
  ngOnInit(): void {
    this.accountingService.GetPrimaryAccounts().subscribe({
      next: (accounts) => (this.primaryAccounts = accounts),
    });
  }

  GetAccountChild(id, e) {
    const targetElement = e.target as HTMLDetailsElement;
    if (!targetElement.open) {
      targetElement.querySelectorAll('.child').forEach((el) => el.remove());
      return;
    }
    this.accountingService.GetChilds(id).subscribe({
      next: (accounts) => {
        accounts.forEach((el) => {
          const summary = document.createElement('summary');
          summary.innerHTML = `${el.name} | ${el.number}`;

          let child = document.createElement('details');
          child.className = 'my-3  cursor-pointer text-xl child font-medium';
          child.style.marginLeft = this.marginLeft * el.level + 'px';

          child.addEventListener('toggle', (e) => {
            e.stopPropagation();
            this.GetAccountChild(el.id, e);
          });

          child.append(summary);
          targetElement.appendChild(child);
        });
      },
    });
  }

  //    GetAccountChild(id, e) {}
}
