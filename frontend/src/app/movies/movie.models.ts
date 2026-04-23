export type MovieStatus = 0 | 1;
export type MovieSortBy = 0 | 1 | 2;
export type SortDirection = 0 | 1;

export interface Movie {
  id: string;
  title: string | null;
  status: MovieStatus;
  year: string | null;
  genres: string[] | null;
  notes: string | null;
  rating: number | null;
  watchedDate: string | null;
}

export interface MoviesPage {
  items: Movie[] | null;
  nextCursor: string | null;
  hasMore: boolean;
}

export interface MovieRow {
  id: string | null;
  title: string;
  status: MovieStatus;
  year: string;
  genres: string;
  notes: string;
  rating: number | null;
  watchedDate: string;
  dirty: boolean;
  isNew: boolean;
}

export interface MoviePayload {
  title: string;
  status: MovieStatus;
  year: string | null;
  genres: string[];
  notes: string | null;
  rating: number | null;
  watchedDate: string | null;
}

export interface ProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

export interface MoviesQuery {
  limit: number;
  cursor?: string | null;
  sortBy: MovieSortBy;
  sortDirection: SortDirection;
}
