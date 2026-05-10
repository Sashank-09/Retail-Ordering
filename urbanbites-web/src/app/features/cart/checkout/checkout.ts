import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CartService } from '../../../core/services/cart.service';
import { OrderService } from '../../../core/services/order.service';
import { PromotionService } from '../../../core/services/promotion.service';
import { LoyaltyService } from '../../../core/services/loyalty.service';
import { Cart } from '../../../core/models/cart.models';
import { ApplyCouponResponse } from '../../../core/models/promotion.models';
import { LoyaltyBalance } from '../../../core/models/loyalty.models';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule,FormsModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class CheckoutComponent implements OnInit {

  private fb               = inject(FormBuilder);
  private cartService      = inject(CartService);
  private orderService     = inject(OrderService);
  private promotionService = inject(PromotionService);
  private loyaltyService   = inject(LoyaltyService);
  private router           = inject(Router);

  cart:           Cart | null             = null;
  couponResult:   ApplyCouponResponse | null = null;
  loyaltyBalance: LoyaltyBalance | null   = null;

  isLoading       = true;
  isPlacingOrder  = false;
  isApplyingCoupon = false;
  couponError     = '';
  orderError      = '';

  couponCode      = '';
  useLoyalty      = false;

  checkoutForm: FormGroup;

  constructor() {
    this.checkoutForm = this.fb.group({
      deliveryAddress:  ['', [Validators.required, Validators.minLength(10)]],
      specialRequests:  ['']
    });
  }

  ngOnInit(): void {
    this.loadCart();
    this.loadLoyalty();
  }

  private loadCart(): void {
    this.isLoading = true;
    this.cartService.loadCart().subscribe({
      next: (cart) => {
        this.cart      = cart;
        this.isLoading = false;
        if (!cart || cart.items.length === 0) {
          this.router.navigate(['/cart']);
        }
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['/cart']);
      }
    });
  }

  private loadLoyalty(): void {
    this.loyaltyService.getBalance().subscribe({
      next: (balance) => { this.loyaltyBalance = balance; },
      error: () => {}
    });
  }

  applyCoupon(): void {
    if (!this.couponCode.trim()) return;
    this.isApplyingCoupon = true;
    this.couponError      = '';

    this.promotionService.applyCoupon({
      code:        this.couponCode.toUpperCase(),
      orderAmount: this.cartSubtotal
    }).subscribe({
      next: (result) => {
        this.couponResult    = result;
        this.isApplyingCoupon = false;
      },
      error: (err) => {
        this.couponError      = err.error?.message || 'Invalid coupon code.';
        this.couponResult     = null;
        this.isApplyingCoupon = false;
      }
    });
  }

  removeCoupon(): void {
    this.couponResult = null;
    this.couponCode   = '';
    this.couponError  = '';
  }

  toggleLoyalty(): void {
    this.useLoyalty = !this.useLoyalty;
  }

  placeOrder(): void {
    if (this.checkoutForm.invalid) {
      this.checkoutForm.markAllAsTouched();
      return;
    }

    this.isPlacingOrder = true;
    this.orderError     = '';

    this.orderService.placeOrder({
      deliveryAddress: this.checkoutForm.value.deliveryAddress,
      specialRequests: this.checkoutForm.value.specialRequests || '',
      couponCode:      this.couponResult ? this.couponCode : undefined,
      useLoyaltyPoints: this.useLoyalty
    }).subscribe({
      next: (order) => {
        this.cartService.clearLocalCart();
        this.router.navigate(['/my-bookings', order.id]);
      },
      error: (err) => {
        this.isPlacingOrder = false;
        this.orderError     = err.error?.message || 'Failed to place order.';
      }
    });
  }

  get cartSubtotal(): number {
    return this.cart?.totalAmount ?? 0;
  }

  get discountAmount(): number {
    let discount = this.couponResult?.discountAmount ?? 0;
    if (this.useLoyalty && this.loyaltyBalance) {
      discount += this.loyaltyBalance.equivalentAmount;
    }
    return discount;
  }

  get finalTotal(): number {
    return Math.max(0, this.cartSubtotal - this.discountAmount);
  }

  get loyaltyDiscount(): number {
    return this.loyaltyBalance?.equivalentAmount ?? 0;
  }

  get deliveryAddress() {
    return this.checkoutForm.get('deliveryAddress')!;
  }
}