import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { CartService } from '../../../core/services/cart.service';
import { AuthService } from '../../../core/services/auth.service';
import { Product, Brand, Category } from '../../../core/models/catalogue.models';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.css'
})
export class ProductListComponent implements OnInit {

  private productService = inject(ProductService);
  private cartService    = inject(CartService);
  private authService    = inject(AuthService);
  private router         = inject(Router);
  private route          = inject(ActivatedRoute);

  allProducts:  Product[]  = [];
  products:     Product[]  = [];
  brands:       Brand[]    = [];
  categories:   Category[] = [];

  isLoading        = true;
  searchQuery      = '';
  selectedBrandId  = '';
  selectedCategory = '';
  sortBy           = 'default';

  addingToCart: { [key: string]: boolean } = {};
  addedToCart:  { [key: string]: boolean } = {};

  ngOnInit(): void {
    this.loadBrands();
    this.loadCategories();
    this.loadProducts();

    // Read query params (from brand filter on home page)
    this.route.queryParams.subscribe(params => {
      if (params['brand']) {
        this.searchQuery = params['brand'];
        this.applyFilters();
      }
    });
  }

  private loadProducts(): void {
    this.isLoading = true;
    this.productService.getAllProducts().subscribe({
      next: (products) => {
        this.allProducts = products;
        this.products    = products;
        this.isLoading   = false;
        this.applyFilters();
      },
      error: () => { this.isLoading = false; }
    });
  }

  private loadBrands(): void {
    this.productService.getAllBrands().subscribe({
      next: (brands) => { this.brands = brands; }
    });
  }

  private loadCategories(): void {
    this.productService.getAllCategories().subscribe({
      next: (categories) => { this.categories = categories; }
    });
  }

  onSearch(): void {
    if (this.searchQuery.trim().length > 2) {
      this.isLoading = true;
      this.productService.searchProducts(this.searchQuery).subscribe({
        next: (products) => {
          this.products  = products;
          this.isLoading = false;
        },
        error: () => { this.isLoading = false; }
      });
    } else if (this.searchQuery.trim() === '') {
      this.applyFilters();
    }
  }

  onBrandChange(): void {
    if (this.selectedBrandId) {
      this.isLoading = true;
      this.productService.getProductsByBrand(this.selectedBrandId).subscribe({
        next: (products) => {
          this.products  = this.sortProducts(products);
          this.isLoading = false;
        },
        error: () => { this.isLoading = false; }
      });
    } else {
      this.applyFilters();
    }
  }

  onCategoryChange(): void {
    if (this.selectedCategory) {
      this.isLoading = true;
      this.productService.getProductsByCategory(this.selectedCategory).subscribe({
        next: (products) => {
          this.products  = this.sortProducts(products);
          this.isLoading = false;
        },
        error: () => { this.isLoading = false; }
      });
    } else {
      this.applyFilters();
    }
  }

  applyFilters(): void {
    let filtered = [...this.allProducts];

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase();
      filtered = filtered.filter(p =>
        p.name.toLowerCase().includes(q) ||
        p.brandName.toLowerCase().includes(q) ||
        p.categoryName.toLowerCase().includes(q)
      );
    }

    this.products = this.sortProducts(filtered);
  }

  private sortProducts(products: Product[]): Product[] {
    switch (this.sortBy) {
      case 'price-asc':  return [...products].sort((a, b) => a.price - b.price);
      case 'price-desc': return [...products].sort((a, b) => b.price - a.price);
      case 'name':       return [...products].sort((a, b) =>
                           a.name.localeCompare(b.name));
      default:           return products;
    }
  }

  onSortChange(): void {
    this.products = this.sortProducts(this.products);
  }

  clearFilters(): void {
    this.searchQuery      = '';
    this.selectedBrandId  = '';
    this.selectedCategory = '';
    this.sortBy           = 'default';
    this.products         = this.allProducts;
  }

  addToCart(product: Product, event: Event): void {
    event.stopPropagation();

    // If not logged in, store in guest (localStorage) cart — no redirect
    if (!this.authService.isLoggedIn()) {
      this.addingToCart[product.id] = true;
      this.cartService.addToGuestCart(
        { id: product.id, name: product.name, imageUrl: product.imageUrl, price: product.price },
        1
      );
      this.addingToCart[product.id] = false;
      this.addedToCart[product.id]  = true;
      setTimeout(() => { this.addedToCart[product.id] = false; }, 2000);
      return;
    }

    this.addingToCart[product.id] = true;
    this.cartService.addToCart({ productId: product.id, quantity: 1 }).subscribe({
      next: () => {
        this.addingToCart[product.id] = false;
        this.addedToCart[product.id]  = true;
        setTimeout(() => { this.addedToCart[product.id] = false; }, 2000);
      },
      error: () => { this.addingToCart[product.id] = false; }
    });
  }

  viewProduct(id: string): void {
    this.router.navigate(['/products', id]);
  }

  get hasActiveFilters(): boolean {
    return !!(this.searchQuery || this.selectedBrandId || this.selectedCategory);
  }
}