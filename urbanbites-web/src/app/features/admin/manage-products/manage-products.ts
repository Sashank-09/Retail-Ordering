import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import {
  Product,
  Brand,
  Category,
  CreateProductRequest,
  UpdateProductRequest,
} from '../../../core/models/catalogue.models';
import { forkJoin } from 'rxjs';

type FormMode = 'create' | 'edit' | null;

@Component({
  selector: 'app-manage-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-products.html',
  styleUrls: ['./manage-products.css'],
})
export class ManageProductsComponent implements OnInit {
  products: Product[] = [];
  filteredProducts: Product[] = [];
  brands: Brand[] = [];
  categories: Category[] = [];
  filteredCategories: Category[] = [];

  isLoading = true;
  error = '';
  searchTerm = '';
  selectedBrandFilter = '';

  // Modal state
  formMode: FormMode = null;
  selectedProduct: Product | null = null;
  isSaving = false;
  saveError = '';
  deleteError = '';
  deletingId: string | null = null;

  form: CreateProductRequest = this.emptyForm();

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.isLoading = true;
    this.error = '';

    forkJoin({
      products: this.productService.getAllProducts(),
      brands: this.productService.getAllBrands(),
      categories: this.productService.getAllCategories(),
    }).subscribe({
      next: ({ products, brands, categories }) => {
        this.products = products;
        this.brands = brands;
        this.categories = categories;
        this.filteredCategories = categories;
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load products:', err);
        this.error = 'Failed to load products.';
        this.isLoading = false;
      },
    });
  }

  applyFilters(): void {
    let result = [...this.products];

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.trim().toLowerCase();
      result = result.filter(
        (p) =>
          p.name.toLowerCase().includes(term) ||
          p.brandName?.toLowerCase().includes(term) ||
          p.categoryName?.toLowerCase().includes(term),
      );
    }

    if (this.selectedBrandFilter) {
      result = result.filter((p) => p.brandName === this.selectedBrandFilter);
    }

    this.filteredProducts = result;
  }

  onBrandChange(): void {
    // Filter categories when brand changes in the form
    const brand = this.brands.find((b) => b.id === this.form.brandId);
    if (brand) {
      this.productService.getCategoriesByBrand(brand.id).subscribe({
        next: (cats) => (this.filteredCategories = cats),
        error: () => (this.filteredCategories = this.categories),
      });
    } else {
      this.filteredCategories = this.categories;
    }
    this.form.categoryId = '';
  }

  openCreate(): void {
    this.formMode = 'create';
    this.selectedProduct = null;
    this.form = this.emptyForm();
    this.filteredCategories = this.categories;
    this.saveError = '';
  }

  openEdit(product: Product): void {
    this.formMode = 'edit';
    this.selectedProduct = product;
    this.saveError = '';

    // Find matching brand/category IDs
    const brand = this.brands.find((b) => b.name === product.brandName);
    const category = this.categories.find((c) => c.name === product.categoryName);

    this.form = {
      name: product.name,
      description: product.description,
      price: product.price,
      stockCount: product.stockCount,
      imageUrl: product.imageUrl,
      packaging: product.packaging,
      brandId: brand?.id ?? '',
      categoryId: category?.id ?? '',
    };

    if (brand) {
      this.productService.getCategoriesByBrand(brand.id).subscribe({
        next: (cats) => (this.filteredCategories = cats),
        error: () => (this.filteredCategories = this.categories),
      });
    }
  }

  closeModal(): void {
    this.formMode = null;
    this.selectedProduct = null;
    this.saveError = '';
  }

  saveProduct(): void {
    if (this.isSaving) return;
    this.isSaving = true;
    this.saveError = '';

    if (this.formMode === 'create') {
      this.productService.createProduct(this.form).subscribe({
        next: (created) => {
          this.products.unshift(created);
          this.applyFilters();
          this.isSaving = false;
          this.closeModal();
        },
        error: (err) => {
          this.saveError = err?.error?.message ?? 'Failed to create product.';
          this.isSaving = false;
        },
      });
    } else if (this.formMode === 'edit' && this.selectedProduct) {
      const updateRequest: UpdateProductRequest = {
        name: this.form.name,
        description: this.form.description,
        price: this.form.price,
        stockCount: this.form.stockCount,
        imageUrl: this.form.imageUrl,
        packaging: this.form.packaging,
      };

      this.productService.updateProduct(this.selectedProduct.id, updateRequest).subscribe({
        next: (updated) => {
          const idx = this.products.findIndex((p) => p.id === this.selectedProduct!.id);
          if (idx !== -1) this.products[idx] = updated;
          this.applyFilters();
          this.isSaving = false;
          this.closeModal();
        },
        error: (err) => {
          this.saveError = err?.error?.message ?? 'Failed to update product.';
          this.isSaving = false;
        },
      });
    }
  }

  deleteProduct(id: string): void {
    if (!confirm('Are you sure you want to delete this product?')) return;
    this.deletingId = id;
    this.deleteError = '';

    this.productService.deleteProduct(id).subscribe({
      next: () => {
        this.products = this.products.filter((p) => p.id !== id);
        this.applyFilters();
        this.deletingId = null;
      },
      error: (err) => {
        this.deleteError = err?.error?.message ?? 'Failed to delete product.';
        this.deletingId = null;
      },
    });
  }

  private emptyForm(): CreateProductRequest {
    return {
      name: '',
      description: '',
      price: 0,
      stockCount: 0,
      imageUrl: '',
      packaging: '',
      categoryId: '',
      brandId: '',
    };
  }

  trackById(_: number, item: Product): string {
    return item.id;
  }
}
