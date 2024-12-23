import { Component, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-search',
  imports: [],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css',
})
export class SearchComponent {
  @Output() Search = new EventEmitter<string>();

  Seach(key: HTMLInputElement) {
    this.Search.emit(key.value);
  }
}
