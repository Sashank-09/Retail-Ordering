import { Component, OnInit, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { CartService } from '../../core/services/cart.service';
import { AuthService } from '../../core/services/auth.service';
import { Cart, CartItem } from '../../core/models/cart.models';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.css'
})
export class CartComponent implements OnInit {

  private cartService = inject(CartService);
  private authService = inject(AuthService);
  private router      = inject(Router);

  cart:      Cart | null = null;
  isLoading  = true;
  updatingId = '';
  removingId = '';
  isClearing = false;

  /** True when the currently displayed cart is the guest (localStorage) cart */
  get isGuestCart(): boolean { return !this.authService.isLoggedIn(); }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.loadServerCart();
    } else {
      // Show guest cart immediately — no HTTP call needed
      this.cart      = this.cartService.guestCart();
      this.isLoading = false;
    }
  }

  private loadServerCart(): void {
    this.isLoading = true;
    this.cartService.loadCart().subscribe({
      next: (cart) => {
        this.cart      = cart;
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }

  // ─── Authenticated cart operations ──────────────────────────────────────────

  increaseQty(item: CartItem): void {
    if (this.isGuestCart) {
      this.cartService.updateGuestItem(item.productId, item.quantity + 1);
      this.cart = this.cartService.guestCart();
      return;
    }
    this.updatingId = item.cartItemId;
    this.cartService.updateItem(item.cartItemId, {
      quantity: item.quantity + 1
    }).subscribe({
      next: (cart) => { this.cart = cart; this.updatingId = ''; },
      error: ()    => { this.updatingId = ''; }
    });
  }

  decreaseQty(item: CartItem): void {
    if (this.isGuestCart) {
      if (item.quantity <= 1) { this.removeItem(item); return; }
      this.cartService.updateGuestItem(item.productId, item.quantity - 1);
      this.cart = this.cartService.guestCart();
      return;
    }
    if (item.quantity <= 1) { this.removeItem(item); return; }
    this.updatingId = item.cartItemId;
    this.cartService.updateItem(item.cartItemId, {
      quantity: item.quantity - 1
    }).subscribe({
      next: (cart) => { this.cart = cart; this.updatingId = ''; },
      error: ()    => { this.updatingId = ''; }
    });
  }

  removeItem(item: CartItem): void {
    if (this.isGuestCart) {
      this.cartService.removeGuestItem(item.productId);
      this.cart = this.cartService.guestCart();
      return;
    }
    this.removingId = item.cartItemId;
    this.cartService.removeItem(item.cartItemId).subscribe({
      next: () => { this.removingId = ''; this.loadServerCart(); },
      error: () => { this.removingId = ''; }
    });
  }

  clearCart(): void {
    if (this.isGuestCart) {
      this.cartService.clearGuestCart();
      this.cart = null;
      return;
    }
    this.isClearing = true;
    this.cartService.clearCart().subscribe({
      next: () => { this.cart = null; this.isClearing = false; },
      error: () => { this.isClearing = false; }
    });
  }

  /**
   * Proceed to checkout.
   * If the user is not logged in, redirect to login with returnUrl=/cart
   * so they come back here and the guest cart is shown, then checkout.
   */
  proceedToCheckout(): void {
    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/auth/login'], { queryParams: { returnUrl: '/cart' } });
      return;
    }
    this.router.navigate(['/checkout']);
  }

  get isEmpty(): boolean {
    return !this.cart || this.cart.items.length === 0;
  }

  get itemCount(): number {
    return this.cart?.items.reduce((s, i) => s + i.quantity, 0) ?? 0;
  }
}