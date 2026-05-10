export interface CartItem {
  cartItemId: string;
  productId: string;
  productName: string;
  imageUrl: string;
  unitPrice: number;
  quantity: number;
  subTotal: number;
}

export interface Cart {
  id: string;
  items: CartItem[];
  totalAmount: number;
}

export interface AddToCartRequest {
  productId: string;
  quantity: number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}
