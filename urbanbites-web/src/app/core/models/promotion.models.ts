export interface Promotion {
  id: string;
  title: string;
  description: string;
  discountPercent: number;
  startDate: string;
  endDate: string;
  imageUrl: string;
  isActive: boolean;
}

export interface Coupon {
  id: string;
  code: string;
  description: string;
  discountPercent: number;
  expiryDate: string;
  minOrderAmount: number;
}

export interface ApplyCouponRequest {
  code: string;
  orderAmount: number;
}

export interface ApplyCouponResponse {
  code: string;
  discountPercent: number;
  discountAmount: number;
  finalAmount: number;
}
