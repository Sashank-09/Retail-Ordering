import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PromotionService } from '../../../core/services/promotion.service';
import { Coupon } from '../../../core/models/promotion.models';

@Component({
  selector: 'app-manage-coupons',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './manage-coupons.html',
  styleUrls: ['./manage-coupons.css'],
})
export class ManageCouponsComponent implements OnInit {
  coupons: Coupon[] = [];
  isLoading = true;
  error = '';

  // Form state
  showForm = false;
  isSaving = false;
  saveError = '';
  deletingId: string | null = null;
  deleteError = '';

  form: Partial<Coupon> = this.emptyForm();

  constructor(private promotionService: PromotionService) {}

  ngOnInit(): void {
    this.loadCoupons();
  }

  loadCoupons(): void {
    this.isLoading = true;
    this.error = '';

    this.promotionService.getAllCoupons().subscribe({
      next: (coupons) => {
        this.coupons = coupons;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load coupons:', err);
        this.error = 'Failed to load coupons.';
        this.isLoading = false;
      },
    });
  }

  openForm(): void {
    this.form = this.emptyForm();
    this.saveError = '';
    this.showForm = true;
  }

  closeForm(): void {
    this.showForm = false;
    this.saveError = '';
  }

  saveCoupon(): void {
    if (this.isSaving) return;
    this.isSaving = true;
    this.saveError = '';

    this.promotionService.createCoupon(this.form).subscribe({
      next: (created) => {
        this.coupons.unshift(created);
        this.isSaving = false;
        this.closeForm();
      },
      error: (err) => {
        this.saveError = err?.error?.message ?? 'Failed to create coupon.';
        this.isSaving = false;
      },
    });
  }

  deleteCoupon(id: string): void {
    if (!confirm('Are you sure you want to delete this coupon?')) return;
    this.deletingId = id;
    this.deleteError = '';

    this.promotionService.deleteCoupon(id).subscribe({
      next: () => {
        this.coupons = this.coupons.filter((c) => c.id !== id);
        this.deletingId = null;
      },
      error: (err) => {
        this.deleteError = err?.error?.message ?? 'Failed to delete coupon.';
        this.deletingId = null;
      },
    });
  }

  isExpired(expiryDate: string): boolean {
    return new Date(expiryDate) < new Date();
  }

  private emptyForm(): Partial<Coupon> {
    return {
      code: '',
      description: '',
      discountPercent: 0,
      minOrderAmount: 0,
      expiryDate: '',
    };
  }

  trackById(_: number, coupon: Coupon): string {
    return coupon.id;
  }
}
