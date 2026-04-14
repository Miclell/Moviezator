import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { MoviePayload, MoviesPage } from './movie.models';

@Injectable({ providedIn: 'root' })
export class MoviesApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/v1/movies';

  getAll(limit: number, cursor?: string | null) {
    const params: Record<string, string> = { Limit: limit.toString() };

    if (cursor) {
      params['Cursor'] = cursor;
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
