import { Component } from '@angular/core';

import { MoviesPageComponent } from './movies/movies-page.component';

@Component({
  selector: 'app-root',
  imports: [MoviesPageComponent],
  template: '<app-movies-page />'
})
export class AppComponent {
}
