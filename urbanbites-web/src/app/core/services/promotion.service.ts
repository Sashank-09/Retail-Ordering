import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Promotion,
  Coupon,
  ApplyCouponRequest,
  ApplyCouponResponse,
} from '../models/promotion.models';

@Injectable({ providedIn: 'root' })
export class PromotionService {
  private readonly base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getActivePromotions(): Observable<Promotion[]> {
    return this.http.get<Promotion[]>(`${this.base}/promotion`);
  }

  getAllPromotions(): Observable<Promotion[]> {
    return this.http.get<Promotion[]>(`${this.base}/promotion/all`);
  }

  createPromotion(promotion: Partial<Promotion>): Observable<Promotion> {
    return this.http.post<Promotion>(`${this.base}/promotion`, promotion);
  }

  deletePromotion(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.base}/promotion/${id}`);
  }

  applyCoupon(request: ApplyCouponRequest): Observable<ApplyCouponResponse> {
    return this.http.post<ApplyCouponResponse>(`${this.base}/coupon/apply`, request);
  }

  getAllCoupons(): Observable<Coupon[]> {
    return this.http.get<Coupon[]>(`${this.base}/coupon`);
  }

  createCoupon(coupon: Partial<Coupon>): Observable<Coupon> {
    return this.http.post<Coupon>(`${this.base}/coupon`, coupon);
  }

  deleteCoupon(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.base}/coupon/${id}`);
  }
}
