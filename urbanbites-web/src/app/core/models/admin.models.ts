export interface DashboardStats {
  totalOrders: number;
  totalRevenue: number;
  totalProducts: number;
  totalCustomers: number;
  pendingOrders: number;
  recentOrders: RecentOrder[];
}

export interface RecentOrder {
  id: string;
  totalAmount: number;
  placedAt: string;
  status: string;
  itemCount: number;
}
