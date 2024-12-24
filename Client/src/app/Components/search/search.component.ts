import { NgClass } from '@angular/common';
import { Component, EventEmitter, Input, input, Output } from '@angular/core';

@Component({
  selector: 'app-search',
  imports: [NgClass],
  templateUrl: './search.component.html',
  styleUrl: './search.component.css',
})
export class SearchComponent {
  @Output() Search = new EventEmitter<string>();
  @Input() Amount;

  Seach(key: HTMLInputElement) {
    this.Search.emit(key.value);
  }
}
