export interface OrderItem {
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  subTotal: number;
}

export interface Order {
  id: string;
  status: string;
  totalAmount: number;
  discountAmount: number;
  deliveryAddress: string;
  specialRequests: string;
  couponCode: string | null;
  loyaltyPointsEarned: number;
  loyaltyPointsUsed: number;
  placedAt: string;
  items: OrderItem[];
}

export interface PlaceOrderRequest {
  deliveryAddress: string;
  specialRequests: string;
  couponCode?: string;
  useLoyaltyPoints: boolean;
}

export interface UpdateOrderStatusRequest {
  status: string;
}

export const ORDER_STATUSES = [
  'Pending',
  'Confirmed',
  'Preparing',
  'OutForDelivery',
  'Delivered',
  'Cancelled',
] as const;

export type OrderStatus = (typeof ORDER_STATUSES)[number];
