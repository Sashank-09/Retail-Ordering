import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Product, Brand, Category,
  CreateProductRequest, UpdateProductRequest,
  CreateBrandRequest, CreateCategoryRequest
} from '../models/catalogue.models';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private readonly base = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAllProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.base}/product`);
  }

  getProductById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.base}/product/${id}`);
  }

  getProductsByCategory(categoryId: string): Observable<Product[]> {
    return this.http.get<Product[]>
      (`${this.base}/product/by-category/${categoryId}`);
  }

  getProductsByBrand(brandId: string): Observable<Product[]> {
    return this.http.get<Product[]>
      (`${this.base}/product/by-brand/${brandId}`);
  }

  searchProducts(query: string): Observable<Product[]> {
    return this.http.get<Product[]>
      (`${this.base}/product/search?q=${query}`);
  }

  createProduct(request: CreateProductRequest): Observable<Product> {
    return this.http.post<Product>(`${this.base}/product`, request);
  }

  updateProduct(id: string, request: UpdateProductRequest): Observable<Product> {
    return this.http.put<Product>(`${this.base}/product/${id}`, request);
  }

  deleteProduct(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>
      (`${this.base}/product/${id}`);
  }

  getAllBrands(): Observable<Brand[]> {
    return this.http.get<Brand[]>(`${this.base}/brand`);
  }

  createBrand(request: CreateBrandRequest): Observable<Brand> {
    return this.http.post<Brand>(`${this.base}/brand`, request);
  }

  updateBrand(id: string, request: CreateBrandRequest): Observable<Brand> {
    return this.http.put<Brand>(`${this.base}/brand/${id}`, request);
  }

  deleteBrand(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.base}/brand/${id}`);
  }

  getAllCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.base}/category`);
  }

  getCategoriesByBrand(brandId: string): Observable<Category[]> {
    return this.http.get<Category[]>
      (`${this.base}/category/by-brand/${brandId}`);
  }

  createCategory(request: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(`${this.base}/category`, request);
  }

  deleteCategory(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>
      (`${this.base}/category/${id}`);
  }
}