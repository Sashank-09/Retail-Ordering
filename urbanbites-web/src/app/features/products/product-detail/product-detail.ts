import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { CartService } from '../../../core/services/cart.service';
import { AuthService } from '../../../core/services/auth.service';
import { Product } from '../../../core/models/catalogue.models';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.css'
})
export class ProductDetailComponent implements OnInit {

  private route          = inject(ActivatedRoute);
  private router         = inject(Router);
  private productService = inject(ProductService);
  private cartService    = inject(CartService);
  private authService    = inject(AuthService);

  product:          Product | null = null;
  relatedProducts:  Product[]      = [];
  isLoading         = true;
  quantity          = 1;
  isAddingToCart    = false;
  addedToCart       = false;
  errorMessage      = '';

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.loadProduct(params['id']);
    });
  }

  private loadProduct(id: string): void {
    this.isLoading = true;
    this.productService.getProductById(id).subscribe({
      next: (product) => {
        this.product   = product;
        this.isLoading = false;
        this.loadRelated(product.categoryName);
      },
      error: () => {
        this.isLoading = false;
        this.router.navigate(['/products']);
      }
    });
  }

  private loadRelated(categoryName: string): void {
    this.productService.getAllProducts().subscribe({
      next: (products) => {
        this.relatedProducts = products
          .filter(p => p.categoryName === categoryName
                    && p.id !== this.product?.id)
          .slice(0, 4);
      }
    });
  }

  increaseQty(): void {
    if (this.product && this.quantity < this.product.stockCount) {
      this.quantity++;
    }
  }

  decreaseQty(): void {
    if (this.quantity > 1) this.quantity--;
  }

  addToCart(): void {
    if (!this.product) return;

    // If not logged in, store in guest (localStorage) cart — no redirect
    if (!this.authService.isLoggedIn()) {
      this.cartService.addToGuestCart(
        { id: this.product.id, name: this.product.name, imageUrl: this.product.imageUrl, price: this.product.price },
        this.quantity
      );
      this.addedToCart = true;
      setTimeout(() => { this.addedToCart = false; }, 3000);
      return;
    }

    this.isAddingToCart = true;
    this.errorMessage   = '';

    this.cartService.addToCart({
      productId: this.product.id,
      quantity:  this.quantity
    }).subscribe({
      next: () => {
        this.isAddingToCart = false;
        this.addedToCart    = true;
        setTimeout(() => { this.addedToCart = false; }, 3000);
      },
      error: (err) => {
        this.isAddingToCart = false;
        this.errorMessage   = err.error?.message || 'Failed to add to cart.';
      }
    });
  }

  goToCart(): void {
    this.router.navigate(['/cart']);
  }

  viewRelated(id: string): void {
    this.router.navigate(['/products', id]);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}