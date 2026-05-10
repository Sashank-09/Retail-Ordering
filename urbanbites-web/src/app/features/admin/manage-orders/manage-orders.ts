import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { Order, ORDER_STATUSES, OrderStatus, UpdateOrderStatusRequest } from '../../../core/models/order.models';

@Component({
  selector: 'app-manage-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, DatePipe],
  templateUrl: './manage-orders.html',
  styleUrls: ['./manage-orders.css'],
})
export class ManageOrdersComponent implements OnInit {
  orders: Order[] = [];
  filteredOrders: Order[] = [];
  isLoading = true;
  error = '';

  // Filter & search
  searchTerm = '';
  selectedStatus = '';
  readonly orderStatuses = ORDER_STATUSES;

  // Inline status update
  updatingOrderId: string | null = null;
  updateError = '';

  constructor(private orderService: OrderService) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.isLoading = true;
    this.error = '';

    this.orderService.getAllOrders().subscribe({
      next: (orders) => {
        this.orders = orders.sort(
          (a, b) => new Date(b.placedAt).getTime() - new Date(a.placedAt).getTime()
        );
        this.applyFilters();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load orders:', err);
        this.error = 'Failed to load orders.';
        this.isLoading = false;
      },
    });
  }

  applyFilters(): void {
    let result = [...this.orders];

    if (this.searchTerm.trim()) {
      const term = this.searchTerm.trim().toLowerCase();
      result = result.filter(
        (o) =>
          o.id.toLowerCase().includes(term) ||
          o.deliveryAddress?.toLowerCase().includes(term)
      );
    }

    if (this.selectedStatus) {
      result = result.filter((o) => o.status === this.selectedStatus);
    }

    this.filteredOrders = result;
  }

  onSearchChange(): void {
    this.applyFilters();
  }

  onStatusFilterChange(): void {
    this.applyFilters();
  }

  updateOrderStatus(orderId: string, newStatus: string): void {
    if (!newStatus) return;

    this.updatingOrderId = orderId;
    this.updateError = '';

    const request: UpdateOrderStatusRequest = { status: newStatus };

    this.orderService.updateStatus(orderId, request).subscribe({
      next: () => {
        const order = this.orders.find((o) => o.id === orderId);
        if (order) order.status = newStatus;
        this.applyFilters();
        this.updatingOrderId = null;
      },
      error: (err) => {
        console.error('Failed to update status:', err);
        this.updateError = `Failed to update order ${orderId.slice(0, 8).toUpperCase()}.`;
        this.updatingOrderId = null;
      },
    });
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Pending:        '🕐 Pending',
      Confirmed:      '✅ Confirmed',
      Preparing:      '👨‍🍳 Preparing',
      OutForDelivery: '🛵 Out for Delivery',
      Delivered:      '🎉 Delivered',
      Cancelled:      '❌ Cancelled',
    };
    return labels[status] ?? status;
  }

  getStatusClass(status: string): string {
    return 'status-' + status.toLowerCase();
  }

  trackById(_: number, order: Order): string {
    return order.id;
  }
}