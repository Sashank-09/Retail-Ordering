import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, of } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Cart, CartItem, AddToCartRequest, UpdateCartItemRequest } from '../models/cart.models';

const GUEST_CART_KEY = 'urbanbites_guest_cart';

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly apiUrl = `${environment.apiUrl}/cart`;

  private _cart      = signal<Cart | null>(null);
  private _guestCart = signal<Cart | null>(this.loadGuestCart());

  cart       = this._cart.asReadonly();
  guestCart  = this._guestCart.asReadonly();
  itemCount  = computed(() => this._cart()?.items.reduce((sum, i) => sum + i.quantity, 0) ?? 0);
  guestCount = computed(() => this._guestCart()?.items.reduce((sum, i) => sum + i.quantity, 0) ?? 0);
  totalAmount = computed(() => this._cart()?.totalAmount ?? 0);

  constructor(private http: HttpClient) {}

  // ─── Server cart (authenticated) ────────────────────────────────────────────

  loadCart(): Observable<Cart> {
    return this.http.get<Cart>(this.apiUrl).pipe(tap((cart) => this._cart.set(cart)));
  }

  addToCart(request: AddToCartRequest): Observable<Cart> {
    return this.http
      .post<Cart>(`${this.apiUrl}/add`, request)
      .pipe(tap((cart) => this._cart.set(cart)));
  }

  updateItem(cartItemId: string, request: UpdateCartItemRequest): Observable<Cart> {
    return this.http
      .put<Cart>(`${this.apiUrl}/update/${cartItemId}`, request)
      .pipe(tap((cart) => this._cart.set(cart)));
  }

  removeItem(cartItemId: string): Observable<{ message: string }> {
    return this.http
      .delete<{ message: string }>(`${this.apiUrl}/remove/${cartItemId}`)
      .pipe(tap(() => this.loadCart().subscribe()));
  }

  clearCart(): Observable<{ message: string }> {
    return this.http
      .delete<{ message: string }>(`${this.apiUrl}/clear`)
      .pipe(tap(() => this._cart.set(null)));
  }

  clearLocalCart(): void {
    this._cart.set(null);
  }

  // ─── Guest cart (localStorage, no auth required) ─────────────────────────────

  addToGuestCart(product: { id: string; name: string; imageUrl: string; price: number }, quantity: number): void {
    const cart = this._guestCart() ?? this.emptyGuestCart();
    const existing = cart.items.find(i => i.productId === product.id);

    if (existing) {
      existing.quantity += quantity;
      existing.subTotal  = existing.unitPrice * existing.quantity;
    } else {
      const newItem: CartItem = {
        cartItemId:  product.id,          // reuse productId as key for guest
        productId:   product.id,
        productName: product.name,
        imageUrl:    product.imageUrl,
        unitPrice:   product.price,
        quantity,
        subTotal:    product.price * quantity
      };
      cart.items.push(newItem);
    }

    cart.totalAmount = cart.items.reduce((s, i) => s + i.subTotal, 0);
    this.saveGuestCart(cart);
  }

  updateGuestItem(productId: string, quantity: number): void {
    const cart = this._guestCart();
    if (!cart) return;
    const item = cart.items.find(i => i.productId === productId);
    if (!item) return;
    if (quantity <= 0) {
      this.removeGuestItem(productId);
      return;
    }
    item.quantity = quantity;
    item.subTotal = item.unitPrice * quantity;
    cart.totalAmount = cart.items.reduce((s, i) => s + i.subTotal, 0);
    this.saveGuestCart(cart);
  }

  removeGuestItem(productId: string): void {
    const cart = this._guestCart();
    if (!cart) return;
    cart.items = cart.items.filter(i => i.productId !== productId);
    cart.totalAmount = cart.items.reduce((s, i) => s + i.subTotal, 0);
    this.saveGuestCart(cart.items.length > 0 ? cart : null);
  }

  clearGuestCart(): void {
    this.saveGuestCart(null);
  }

  /** Returns guest cart items that should be synced after login. */
  getGuestCartItems(): CartItem[] {
    return this._guestCart()?.items ?? [];
  }

  /** Pushes all guest-cart items to the server cart, then clears the guest cart. */
  mergeGuestCartToServer(): Observable<Cart | null> {
    const items = this.getGuestCartItems();
    if (items.length === 0) return of(null);

    const addAll = (index: number): Observable<Cart | null> => {
      if (index >= items.length) return of(this._cart());
      const item = items[index];
      return this.addToCart({ productId: item.productId, quantity: item.quantity }).pipe(
        tap(() => {
          if (index === items.length - 1) this.clearGuestCart();
        })
      );
    };
    // Chain sequentially — simple fire-and-forget for now
    items.forEach(item =>
      this.addToCart({ productId: item.productId, quantity: item.quantity }).subscribe()
    );
    this.clearGuestCart();
    return of(null);
  }

  // ─── Private helpers ─────────────────────────────────────────────────────────

  private emptyGuestCart(): Cart {
    return { id: 'guest', items: [], totalAmount: 0 };
  }

  private loadGuestCart(): Cart | null {
    try {
      const raw = localStorage.getItem(GUEST_CART_KEY);
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }

  private saveGuestCart(cart: Cart | null): void {
    if (cart && cart.items.length > 0) {
      localStorage.setItem(GUEST_CART_KEY, JSON.stringify(cart));
    } else {
      localStorage.removeItem(GUEST_CART_KEY);
    }
    this._guestCart.set(cart && cart.items.length > 0 ? cart : null);
  }
}
