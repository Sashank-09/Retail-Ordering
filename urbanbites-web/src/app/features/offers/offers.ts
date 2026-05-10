import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PromotionService } from '../../core/services/promotion.service';
import { Promotion, Coupon } from '../../core/models/promotion.models';
import { LoyaltyService } from '../../core/services/loyalty.service';
import { LoyaltyBalance } from '../../core/models/loyalty.models';

@Component({
  selector: 'app-offers',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './offers.html',
  styleUrl: './offers.css'
})
export class OffersComponent implements OnInit {
  private promotionService = inject(PromotionService);
  private loyaltyService   = inject(LoyaltyService);

  promotions:     Promotion[]          = [];
  coupons:        Coupon[]             = [];
  loyaltyBalance: LoyaltyBalance | null = null;
  isLoading       = true;
  copiedCode      = '';

  ngOnInit(): void {
    Promise.all([
      this.loadPromotions(),
      this.loadCoupons(),
      this.loadLoyalty()
    ]).finally(() => this.isLoading = false);
  }

  private async loadPromotions(): Promise<void> {
    return new Promise(resolve => {
      this.promotionService.getActivePromotions().subscribe({
        next: (data) => { this.promotions = data; resolve(); },
        error: () => resolve()
      });
    });
  }

  private async loadCoupons(): Promise<void> {
    return new Promise(resolve => {
      // Coupons endpoint is admin-only (403), show empty gracefully
      this.promotionService.getAllCoupons().subscribe({
        next: (data) => { this.coupons = data; resolve(); },
        error: () => resolve()
      });
    });
  }

  private async loadLoyalty(): Promise<void> {
    return new Promise(resolve => {
      this.loyaltyService.getBalance().subscribe({
        next: (data) => { this.loyaltyBalance = data; resolve(); },
        error: () => resolve()
      });
    });
  }

  copyCoupon(code: string): void {
    navigator.clipboard.writeText(code).then(() => {
      this.copiedCode = code;
      setTimeout(() => this.copiedCode = '', 2000);
    });
  }
}