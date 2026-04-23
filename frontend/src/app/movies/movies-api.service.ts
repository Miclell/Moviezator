import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { MoviePayload, MoviesPage, MoviesQuery } from './movie.models';

@Injectable({ providedIn: 'root' })
export class MoviesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/movies';

  getAll(query: MoviesQuery) {
    const params: Record<string, string> = {
      Limit: query.limit.toString(),
      SortBy: query.sortBy.toString(),
      SortDirection: query.sortDirection.toString()
    };

    if (query.cursor) {
      params['Cursor'] = query.cursor;
    }

    return this.http.get<MoviesPage>(`${this.baseUrl}/all`, { params });
  }

  create(payload: MoviePayload) {
    return this.http.post<void>(this.baseUrl, payload);
  }

  update(id: string, payload: MoviePayload) {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
