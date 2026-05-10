import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PromotionService } from '../../../core/services/promotion.service';
import { Promotion } from '../../../core/models/promotion.models';

@Component({
  selector: 'app-manage-promotions',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './manage-promotions.html',
  styleUrls: ['./manage-promotions.css'],
})
export class ManagePromotionsComponent implements OnInit {
  promotions: Promotion[] = [];
  isLoading = true;
  error = '';

  // Form state
  showForm = false;
  isSaving = false;
  saveError = '';
  deletingId: string | null = null;
  deleteError = '';

  form: Partial<Promotion> = this.emptyForm();

  constructor(private promotionService: PromotionService) {}

  ngOnInit(): void {
    this.loadPromotions();
  }

  loadPromotions(): void {
    this.isLoading = true;
    this.error = '';

    // getAllPromotions includes inactive ones — ideal for admin view
    this.promotionService.getAllPromotions().subscribe({
      next: (promotions) => {
        this.promotions = promotions.sort(
          (a, b) => new Date(b.startDate).getTime() - new Date(a.startDate).getTime(),
        );
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load promotions:', err);
        this.error = 'Failed to load promotions.';
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

  savePromotion(): void {
    if (this.isSaving) return;
    this.isSaving = true;
    this.saveError = '';

    this.promotionService.createPromotion(this.form).subscribe({
      next: (created) => {
        this.promotions.unshift(created);
        this.isSaving = false;
        this.closeForm();
      },
      error: (err) => {
        this.saveError = err?.error?.message ?? 'Failed to create promotion.';
        this.isSaving = false;
      },
    });
  }

  deletePromotion(id: string): void {
    if (!confirm('Are you sure you want to delete this promotion?')) return;
    this.deletingId = id;
    this.deleteError = '';

    this.promotionService.deletePromotion(id).subscribe({
      next: () => {
        this.promotions = this.promotions.filter((p) => p.id !== id);
        this.deletingId = null;
      },
      error: (err) => {
        this.deleteError = err?.error?.message ?? 'Failed to delete promotion.';
        this.deletingId = null;
      },
    });
  }

  isActive(promotion: Promotion): boolean {
    const now = new Date();
    return (
      promotion.isActive &&
      new Date(promotion.startDate) <= now &&
      new Date(promotion.endDate) >= now
    );
  }

  getStatusLabel(promotion: Promotion): string {
    if (!promotion.isActive) return '⛔ Inactive';
    const now = new Date();
    if (new Date(promotion.startDate) > now) return '⏳ Scheduled';
    if (new Date(promotion.endDate) < now) return '🔴 Expired';
    return '🟢 Active';
  }

  private emptyForm(): Partial<Promotion> {
    return {
      title: '',
      description: '',
      discountPercent: 0,
      startDate: '',
      endDate: '',
      imageUrl: '',
      isActive: true,
    };
  }

  trackById(_: number, promotion: Promotion): string {
    return promotion.id;
  }
}
