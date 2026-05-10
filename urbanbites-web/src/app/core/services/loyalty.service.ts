import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LoyaltyBalance } from '../models/loyalty.models';

@Injectable({ providedIn: 'root' })
export class LoyaltyService {
  private readonly apiUrl = `${environment.apiUrl}/loyalty`;

  constructor(private http: HttpClient) {}

  getBalance(): Observable<LoyaltyBalance> {
    return this.http.get<LoyaltyBalance>(`${this.apiUrl}/balance`);
  }
}
