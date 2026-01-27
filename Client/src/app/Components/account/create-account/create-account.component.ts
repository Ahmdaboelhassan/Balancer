import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AccountService } from '../../../Services/account.service';
import { Title } from '@angular/platform-browser';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Account } from '../../../Interfaces/Response/Account';
import { AccountSelectList } from '../../../Interfaces/Response/AccountSelectList';
import { NgFor } from '@angular/common';
import { CreateAccount } from '../../../Interfaces/Request/CreateAccount';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-create-account',
  imports: [ReactiveFormsModule, NgFor, RouterLink],
  templateUrl: './create-account.component.html',
  styleUrl: './create-account.component.css',
})
export class CreateAccountComponent implements OnInit {
  isEdit = false;
  accountForm: FormGroup;
  account: Account;
  accounts: AccountSelectList[];
  constructor(
    private route: ActivatedRoute,
    private accountService: AccountService,
    private titleService: Title,
    private toastr: ToastrService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      if (params['id']) {
        this.isEdit = true;
        this.getAccount(+params['id']);
        this.titleService.setTitle('Edit Account');
      } else {
        this.GetAllAccountSelectList();
        this.titleService.setTitle('Create Account');
      }
    });
  }

  getAccount(id) {
    this.accountService.GetAccount(id).subscribe({
      next: (account) => {
        {
          this.account = account;
          this.accounts = account.accounts;
          this.intializeEditForm();
        }
      },
    });
  }

  GetAllAccountSelectList() {
    this.accountService.GetAllAccountSelectList().subscribe({
      next: (accounts) => {
        this.accounts = accounts;
        this.intializeCreateForm();
      },
    });
  }
  intializeEditForm() {
    debugger;
    this.accountForm = new FormGroup({
      id: new FormControl(this.account.id),
      name: new FormControl(this.account.name, Validators.required),
      description: new FormControl(this.account.description),
      parent: new FormControl(this.account.parentId),
      isArchive: new FormControl(this.account.isArchive),
      number: new FormControl({
        value: this.account.accountNumber,
        disabled: true,
      }),
    });
  }
  intializeCreateForm() {
    this.accountForm = new FormGroup({
      id: new FormControl(0),
      name: new FormControl('', Validators.required),
      description: new FormControl(''),
      parent: new FormControl(0),
      isArchive: new FormControl(false),
      number: new FormControl({
        value: '',
        disabled: true,
      }),
    });
  }
  Submit() {
    if (!this.accountForm.valid) {
      this.toastr.error('From Is Invalid !');
      return;
    }
    const formValue = this.accountForm.value;
    const SaveJournal: CreateAccount = {
      description: formValue.description,
      id: formValue.id,
      name: formValue.name,
      parentId: formValue.parent,
      isArchive: formValue.isArchive,
    };

    if (this.isEdit) {
      this.accountService.EditAccount(SaveJournal).subscribe({
        next: (result) => {
          const id = this.accountForm.get('id').value;
          this.toastr.success(result.message);
          this.router.navigate(['/Account', 'Edit', id]);
        },
        error: (error) => {
          this.toastr.error(error.error.message);
        },
      });
    } else {
      this.accountService.CreateAccount(SaveJournal).subscribe({
        next: (result) => {
          this.toastr.success(result.message);
          this.GetAllAccountSelectList();
        },
        error: (error) => this.toastr.error(error.error.message),
      });
    }
  }
  DeleteAccount() {
    var result = confirm('Are You Sure For Deleting This Account?');
    if (result) {
      this.accountService.DeleteAccount(this.account.id).subscribe({
        next: (result) => {
          this.toastr.success(result.message);
          this.router.navigate(['/Account', 'List']);
        },
        error: (error) => this.toastr.error(error.error.message),
      });
    }
  }
}
