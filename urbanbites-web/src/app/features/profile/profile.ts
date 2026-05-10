import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AuthService } from '../../core/services/auth.service';
import { OrderService } from '../../core/services/order.service';
import { UserProfile, UpdateProfileRequest } from '../../core/models/auth.models';
import { Order } from '../../core/models/order.models';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './profile.html',
  styleUrls: ['./profile.css'],
})
export class ProfileComponent implements OnInit {
  profile: UserProfile | null = null;
  isLoading = true;
  isEditing = false;
  isSaving = false;
  saveSuccess = false;
  saveError = '';

  orderCount = 0;
  loyaltyPoints = 0;

  editForm: UpdateProfileRequest = {
    fullName: '',
    phone: '',
  };

  constructor(
    private authService: AuthService,
    private orderService: OrderService,
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.isLoading = true;

    // Fetch profile and orders in parallel
    forkJoin({
      profile: this.authService.getProfile(),
      orders: this.orderService.getMyOrders(),
    }).subscribe({
      next: ({ profile, orders }) => {
        this.profile = profile;
        this.orderCount = orders.length;
        this.loyaltyPoints = this.calcLoyaltyPoints(orders);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load profile data:', err);
        this.isLoading = false;
      },
    });
  }

  /**
   * Loyalty points = total earned across all orders
   * minus total used across all orders.
   */
  private calcLoyaltyPoints(orders: Order[]): number {
    const earned = orders.reduce((sum, o) => sum + (o.loyaltyPointsEarned ?? 0), 0);
    const used = orders.reduce((sum, o) => sum + (o.loyaltyPointsUsed ?? 0), 0);
    return Math.max(0, earned - used);
  }

  getInitials(): string {
    if (!this.profile?.fullName) return '?';
    return this.profile.fullName
      .split(' ')
      .map((word) => word.charAt(0).toUpperCase())
      .slice(0, 2)
      .join('');
  }

  startEditing(): void {
    if (!this.profile) return;
    this.editForm = {
      fullName: this.profile.fullName,
      phone: this.profile.phone ?? '',
    };
    this.isEditing = true;
    this.saveSuccess = false;
    this.saveError = '';
  }

  cancelEditing(): void {
    this.isEditing = false;
    this.saveSuccess = false;
    this.saveError = '';
  }

  saveProfile(): void {
    if (!this.profile || this.isSaving) return;

    this.isSaving = true;
    this.saveSuccess = false;
    this.saveError = '';

    const payload: UpdateProfileRequest = {
      fullName: this.editForm.fullName.trim(),
      phone: this.editForm.phone.trim(),
    };

    this.authService.updateProfile(payload).subscribe({
      next: () => {
        this.profile = { ...this.profile!, ...payload };
        this.isSaving = false;
        this.isEditing = false;
        this.saveSuccess = true;
        setTimeout(() => (this.saveSuccess = false), 3000);
      },
      error: (err) => {
        console.error('Failed to update profile:', err);
        this.isSaving = false;
        this.saveError = err?.error?.message ?? 'Failed to save. Please try again.';
      },
    });
  }
}
