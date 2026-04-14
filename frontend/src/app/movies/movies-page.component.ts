import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { firstValueFrom } from 'rxjs';

import { Movie, MoviePayload, MovieRow, ProblemDetails } from './movie.models';
import { MoviesApiService } from './movies-api.service';

@Component({
  selector: 'app-movies-page',
  imports: [FormsModule],
  template: `
    <main class="shell">
      <header class="topbar">
        <div>
          <p>Moviezator</p>
          <h1>Movies</h1>
        </div>

        <div class="tools">
          <label>
            Page size
            <input type="number" min="1" max="100" [(ngModel)]="pageSize" (change)="reload()">
          </label>

          <button type="button" class="secondary" (click)="reload()" [disabled]="busy()">Refresh</button>
        </div>
      </header>

      @if (error()) {
        <p class="message error">{{ error() }}</p>
      }

      @if (notice()) {
        <p class="message">{{ notice() }}</p>
      }

      <section class="table-wrap" aria-label="Movies table">
        <table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Status</th>
              <th>Year</th>
              <th>Genres</th>
              <th>Rating</th>
              <th>Watched</th>
              <th>Notes</th>
              <th>Actions</th>
            </tr>
          </thead>

          <tbody>
            <tr class="new-row">
              <td>
                <input aria-label="New movie title" [(ngModel)]="newRow.title" placeholder="Movie title" maxlength="256">
              </td>
              <td>
                <select aria-label="New movie status" [(ngModel)]="newRow.status">
                  <option [ngValue]="1">To watch</option>
                  <option [ngValue]="0">Watched</option>
                </select>
              </td>
              <td>
                <input aria-label="New movie year" [(ngModel)]="newRow.year" type="date">
              </td>
              <td>
                <input aria-label="New movie genres" [(ngModel)]="newRow.genres" placeholder="Drama, Comedy">
              </td>
              <td>
                <input aria-label="New movie rating" [(ngModel)]="newRow.rating" type="number" min="0" max="10" step="0.1">
              </td>
              <td>
                <input aria-label="New movie watched date" [(ngModel)]="newRow.watchedDate" type="date">
              </td>
              <td>
                <input aria-label="New movie notes" [(ngModel)]="newRow.notes" placeholder="Notes" maxlength="4096">
              </td>
              <td>
                <button type="button" (click)="createMovie()" [disabled]="busy() || !newRow.title.trim()">Add</button>
              </td>
            </tr>

            @for (row of rows(); track row.id) {
              <tr>
                <td>
                  <input aria-label="Movie title" [(ngModel)]="row.title" (ngModelChange)="markDirty(row)" maxlength="256">
                </td>
                <td>
                  <select aria-label="Movie status" [(ngModel)]="row.status" (ngModelChange)="markDirty(row)">
                    <option [ngValue]="1">To watch</option>
                    <option [ngValue]="0">Watched</option>
                  </select>
                </td>
                <td>
                  <input aria-label="Movie year" [(ngModel)]="row.year" (ngModelChange)="markDirty(row)" type="date">
                </td>
                <td>
                  <input aria-label="Movie genres" [(ngModel)]="row.genres" (ngModelChange)="markDirty(row)">
                </td>
                <td>
                  <input aria-label="Movie rating" [(ngModel)]="row.rating" (ngModelChange)="markDirty(row)" type="number" min="0" max="10" step="0.1">
                </td>
                <td>
                  <input aria-label="Movie watched date" [(ngModel)]="row.watchedDate" (ngModelChange)="markDirty(row)" type="date">
                </td>
                <td>
                  <input aria-label="Movie notes" [(ngModel)]="row.notes" (ngModelChange)="markDirty(row)" maxlength="4096">
                </td>
                <td>
                  <div class="row-actions">
                    <button type="button" (click)="saveRow(row)" [disabled]="busy() || !row.dirty || !row.title.trim()">Save</button>
                    <button type="button" class="danger" (click)="deleteMovie(row)" [disabled]="busy()">Delete</button>
                  </div>
                </td>
              </tr>
            }

            @if (rows().length === 0 && !busy()) {
              <tr>
                <td colspan="8" class="empty">No movies yet.</td>
              </tr>
            }
          </tbody>
        </table>
      </section>

      <footer class="footer">
        <span>{{ rows().length }} rows</span>

        @if (hasMore()) {
          <button type="button" class="secondary" (click)="loadNextPage()" [disabled]="busy()">Load more</button>
        }
      </footer>
    </main>
  `
})
export class MoviesPageComponent {
  private readonly api = inject(MoviesApiService);

  protected readonly rows = signal<MovieRow[]>([]);
  protected readonly nextCursor = signal<string | null>(null);
  protected readonly hasMore = signal(false);
  protected readonly busy = signal(false);
  protected readonly error = signal('');
  protected readonly notice = signal('');
  protected newRow = this.createEmptyRow(true);
  protected pageSize = 20;

  constructor() {
    void this.reload();
  }

  protected async reload() {
    await this.loadRows(null, true);
  }

  protected async loadNextPage() {
    await this.loadRows(this.nextCursor(), false);
  }

  protected markDirty(row: MovieRow) {
    row.dirty = true;
  }

  protected async createMovie() {
    await this.run(async () => {
      await firstValueFrom(this.api.create(this.toPayload(this.newRow)));
      this.newRow = this.createEmptyRow(true);
      this.notice.set('Added.');
      await this.reload();
    });
  }

  protected async saveRow(row: MovieRow) {
    const id = row.id;

    if (!id) {
      return;
    }

    await this.run(async () => {
      await firstValueFrom(this.api.update(id, this.toPayload(row)));
      row.dirty = false;
      this.notice.set('Saved.');
      await this.reload();
    });
  }

  protected async deleteMovie(row: MovieRow) {
    const id = row.id;

    if (!id) {
      return;
    }

    await this.run(async () => {
      await firstValueFrom(this.api.delete(id));
      this.notice.set('Deleted.');
      await this.reload();
    });
  }

  private async loadRows(cursor: string | null, replace: boolean) {
    await this.run(async () => {
      const page = await firstValueFrom(this.api.getAll(this.normalizedPageSize(), cursor));
      const rows = (page.items ?? []).map(movie => this.toRow(movie));

      this.rows.set(replace ? rows : [...this.rows(), ...rows]);
      this.nextCursor.set(page.nextCursor);
      this.hasMore.set(page.hasMore);
    });
  }

  private async run(action: () => Promise<void>) {
    this.busy.set(true);
    this.error.set('');
    this.notice.set('');

    try {
      await action();
    } catch (err) {
      this.error.set(this.getErrorMessage(err));
    } finally {
      this.busy.set(false);
    }
  }

  private toRow(movie: Movie): MovieRow {
    return {
      id: movie.id,
      title: movie.title ?? '',
      status: movie.status,
      year: this.toDateInput(movie.year),
      genres: movie.genres?.join(', ') ?? '',
      notes: movie.notes ?? '',
      rating: movie.rating,
      watchedDate: this.toDateInput(movie.watchedDate),
      dirty: false,
      isNew: false
    };
  }

  private createEmptyRow(isNew: boolean): MovieRow {
    return {
      id: null,
      title: '',
      status: 1,
      year: '',
      genres: '',
      notes: '',
      rating: null,
      watchedDate: '',
      dirty: false,
      isNew
    };
  }

  private toPayload(row: MovieRow): MoviePayload {
    return {
      title: row.title.trim(),
      status: row.status,
      year: this.toIsoDate(row.year),
      genres: row.genres
        .split(',')
        .map(genre => genre.trim())
        .filter(Boolean),
      notes: row.notes.trim() || null,
      rating: row.rating,
      watchedDate: this.toIsoDate(row.watchedDate)
    };
  }

  private toIsoDate(value: string) {
    return value ? new Date(`${value}T00:00:00.000Z`).toISOString() : null;
  }

  private toDateInput(value: string | null) {
    return value ? value.slice(0, 10) : '';
  }

  private normalizedPageSize() {
    if (!Number.isFinite(this.pageSize)) {
      return 20;
    }

    return Math.min(Math.max(Math.trunc(this.pageSize), 1), 100);
  }

  private getErrorMessage(err: unknown) {
    if (err instanceof HttpErrorResponse) {
      return this.formatHttpError(err);
    }

    if (typeof err === 'object' && err && 'message' in err) {
      return String(err.message);
    }

    return 'Request failed.';
  }

  private formatHttpError(error: HttpErrorResponse) {
    if (error.error && typeof error.error === 'object') {
      const problem = error.error as ProblemDetails;
      const validationErrors = this.formatValidationErrors(problem.errors);

      if (validationErrors) {
        return validationErrors;
      }

      if (problem.detail) {
        return problem.detail;
      }

      if (problem.title) {
        return problem.title;
      }
    }

    if (typeof error.error === 'string' && error.error.trim()) {
      return error.error;
    }

    return `API request failed: ${error.status || 'network error'} ${error.statusText || ''}`.trim();
  }

  private formatValidationErrors(errors: Record<string, string[]> | undefined) {
    if (!errors) {
      return '';
    }

    return Object.entries(errors)
      .flatMap(([field, messages]) => messages.map(message => `${field}: ${message}`))
      .join('\n');
  }
}
