import { Component, OnInit, OnDestroy, inject, DestroyRef } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ProductService } from '../../core/services/product.service';
import { PromotionService } from '../../core/services/promotion.service';
import { AuthService } from '../../core/services/auth.service';

import { Product } from '../../core/models/catalogue.models';
import { Promotion } from '../../core/models/promotion.models';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class HomeComponent implements OnInit, OnDestroy {
  private productService   = inject(ProductService);
  private promotionService = inject(PromotionService);
  private authService      = inject(AuthService);
  private router           = inject(Router);
  private destroyRef       = inject(DestroyRef);

  products: Product[] = [];
  promotions: Promotion[] = [];


  isLoadingProducts = true;
  isLoadingPromotions = true;

  currentSlide = 0;

  private carouselInterval!: any;

  ngOnInit(): void {
    this.loadProducts();
    this.loadPromotions();

  }

  ngOnDestroy(): void {
    if (this.carouselInterval) {
      clearInterval(this.carouselInterval);
    }
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  private loadProducts(): void {
    this.productService.getAllProducts().subscribe({
      next: (products) => {
        this.products = products;
        this.isLoadingProducts = false;
      },

      error: (err) => {
        console.error('Failed to load products', err);
        this.isLoadingProducts = false;
      },
    });
  }

  private loadPromotions(): void {
    this.promotionService
      .getActivePromotions()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (promos) => {
          console.log('PROMOTIONS:', promos);

          this.promotions = promos || [];
          this.isLoadingPromotions = false;

          // ✅ GUARANTEE first slide is visible
          if (this.promotions.length > 0) {
            this.currentSlide = 0;

            // Delay ensures DOM renders before carousel starts
            setTimeout(() => this.startCarousel(), 0);
          }
        },
        error: (err) => {
          console.error('Failed to load promotions', err);
          this.isLoadingPromotions = false;
        },
      });
  }



  private startCarousel(): void {
    if (this.carouselInterval) {
      clearInterval(this.carouselInterval);
    }

    this.carouselInterval = setInterval(() => {
      this.currentSlide = (this.currentSlide + 1) % this.promotions.length;
    }, 4000);
  }

  goToSlide(index: number): void {
    this.currentSlide = index;
  }


  viewProduct(id: string): void {
    this.router.navigate(['/products', id]);
  }



  get featuredProducts(): Product[] {
    return this.products.slice(0, 8);
  }
}
