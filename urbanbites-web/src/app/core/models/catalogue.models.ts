export interface Brand {
  id: string;
  name: string;
  logoUrl: string;
}

export interface CreateBrandRequest {
  name: string;
  logoUrl: string;
}

export interface Category {
  id: string;
  name: string;
  brandName: string;
}

export interface CreateCategoryRequest {
  name: string;
  brandId: string;
}

export interface Product {
  id: string;
  name: string;
  description: string;
  price: number;
  stockCount: number;
  imageUrl: string;
  packaging: string;
  categoryName: string;
  brandName: string;
}

export interface CreateProductRequest {
  name: string;
  description: string;
  price: number;
  stockCount: number;
  imageUrl: string;
  packaging: string;
  categoryId: string;
  brandId: string;
}

export interface UpdateProductRequest {
  name: string;
  description: string;
  price: number;
  stockCount: number;
  imageUrl: string;
  packaging: string;
}
