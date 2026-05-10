import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormsModule } from '@angular/forms';
import { CartService } from '../../../core/services/cart.service';
import { OrderService } from '../../../core/services/order.service';
import { PromotionService } from '../../../core/services/promotion.service';
import { LoyaltyService } from '../../../core/services/loyalty.service';
import { PaymentService } from '../../../core/services/payment.service';
import { Cart } from '../../../core/models/cart.models';
import { ApplyCouponResponse } from '../../../core/models/promotion.models';
import { LoyaltyBalance } from '../../../core/models/loyalty.models';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, RouterLink, ReactiveFormsModule, FormsModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css'
})
export class CheckoutComponent implements OnInit {

  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private orderService = inject(OrderService);
  private promotionService = inject(PromotionService);
  private loyaltyService = inject(LoyaltyService);
  private paymentService = inject(PaymentService);
  private router = inject(Router);

  // You must set this to your Razorpay Test Key ID
  private razorpayKeyId = 'rzp_test_SngL2RZRdt3WjF';

  cart: Cart | null = null;
  couponResult: ApplyCouponResponse | null = null;
  loyaltyBalance: LoyaltyBalance | null = null;

  isLoading = true;
  isPlacingOrder = false;
  isApplyingCoupon = false;
  couponError = '';
  orderError = '';

  couponCode = '';
  useLoyalty = false;

  checkoutForm: FormGroup;

  constructor() {
    this.checkoutForm = this.fb.group({
      deliveryAddress: ['', [Validators.required, Validators.minLength(10)]],
      specialRequests: ['']
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
        this.cart = cart;
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
      error: () => { }
    });
  }

  applyCoupon(): void {
    if (!this.couponCode.trim()) return;
    this.isApplyingCoupon = true;
    this.couponError = '';

    this.promotionService.applyCoupon({
      code: this.couponCode.toUpperCase(),
      orderAmount: this.cartSubtotal
    }).subscribe({
      next: (result) => {
        this.couponResult = result;
        this.isApplyingCoupon = false;
      },
      error: (err) => {
        this.couponError = err.error?.message || 'Invalid coupon code.';
        this.couponResult = null;
        this.isApplyingCoupon = false;
      }
    });
  }

  removeCoupon(): void {
    this.couponResult = null;
    this.couponCode = '';
    this.couponError = '';
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
    this.orderError = '';

    // Step 1: Create Razorpay Order
    this.paymentService.createOrder(this.finalTotal).subscribe({
      next: (res) => {
        // Step 2: Open Razorpay Modal
        const options = {
          key: this.razorpayKeyId,
          amount: this.finalTotal * 100, // Amount in paise
          currency: 'INR',
          name: 'UrbanBites',
          description: 'Food Order Payment',
          order_id: res.razorpayOrderId,
          handler: (response: any) => {
            // Step 3: Verify Signature on Backend
            this.verifyAndCompleteOrder(response.razorpay_payment_id, response.razorpay_order_id, response.razorpay_signature);
          },
          prefill: {
            name: 'Customer',
            email: 'customer@example.com'
          },
          theme: {
            color: '#f97316'
          },
          modal: {
            ondismiss: () => {
              this.isPlacingOrder = false;
              this.orderError = 'Payment was cancelled.';
            }
          }
        };

        const rzp = new (window as any).Razorpay(options);
        rzp.open();
      },
      error: (err) => {
        this.isPlacingOrder = false;
        this.orderError = err.error?.message || 'Failed to initialize payment.';
      }
    });
  }

  private verifyAndCompleteOrder(paymentId: string, orderId: string, signature: string): void {
    this.paymentService.verifyPayment({
      razorpayPaymentId: paymentId,
      razorpayOrderId: orderId,
      razorpaySignature: signature
    }).subscribe({
      next: (verifyRes) => {
        if (verifyRes.success) {
          // Step 4: Place the Order securely
          this.orderService.placeOrder({
            deliveryAddress: this.checkoutForm.value.deliveryAddress,
            specialRequests: this.checkoutForm.value.specialRequests || '',
            couponCode: this.couponResult ? this.couponCode : undefined,
            useLoyaltyPoints: this.useLoyalty,
            transactionId: verifyRes.transactionId
          }).subscribe({
            next: (order) => {
              this.cartService.clearLocalCart();
              this.router.navigate(['/my-bookings', order.id]);
            },
            error: (err) => {
              this.isPlacingOrder = false;
              this.orderError = err.error?.message || 'Failed to create order after payment.';
            }
          });
        }
      },
      error: () => {
        this.isPlacingOrder = false;
        this.orderError = 'Payment verification failed.';
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