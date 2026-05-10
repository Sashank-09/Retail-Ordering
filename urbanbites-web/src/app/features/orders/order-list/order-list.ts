import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../core/models/order.models';

@Component({
  selector: 'app-order-list',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './order-list.html',
  styleUrl: './order-list.css'
})
export class OrderListComponent implements OnInit {
  private orderService = inject(OrderService);
  private router       = inject(Router);

  orders: Order[] = [];
  isLoading = true;

  ngOnInit(): void {
    this.orderService.getMyOrders().subscribe({
      next: (orders) => {
        this.orders   = orders.sort(
          (a, b) => new Date(b.placedAt).getTime() - new Date(a.placedAt).getTime()
        );
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }

  viewOrder(id: string): void {
    this.router.navigate(['/my-bookings', id]);
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
}