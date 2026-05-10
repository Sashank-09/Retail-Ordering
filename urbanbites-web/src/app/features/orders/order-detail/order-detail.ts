import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { OrderService } from '../../../core/services/order.service';
import { Order } from '../../../core/models/order.models';

const STATUS_ORDER = ['Pending', 'Confirmed', 'Preparing', 'OutForDelivery', 'Delivered'];

@Component({
  selector: 'app-order-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './order-detail.html',
  styleUrl: './order-detail.css',
})
export class OrderDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private orderService = inject(OrderService);

  order: Order | null = null;
  isLoading = true;

  statusSteps = [
    { key: 'Pending', label: 'Placed', icon: '🧾' },
    { key: 'Confirmed', label: 'Confirmed', icon: '✅' },
    { key: 'Preparing', label: 'Preparing', icon: '👨‍🍳' },
    { key: 'OutForDelivery', label: 'On the way', icon: '🛵' },
    { key: 'Delivered', label: 'Delivered', icon: '🎉' },
  ];

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.orderService.getOrderById(id).subscribe({
      next: (order) => {
        this.order = order;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      },
    });
  }

  getStatusLabel(status: string): string {
    const labels: Record<string, string> = {
      Pending: '🕐 Pending',
      Confirmed: '✅ Confirmed',
      Preparing: '👨‍🍳 Preparing',
      OutForDelivery: '🛵 Out for Delivery',
      Delivered: '🎉 Delivered',
      Cancelled: '❌ Cancelled',
    };
    return labels[status] ?? status;
  }

  isStepActive(stepKey: string): boolean {
    if (!this.order) return false;
    const currentIdx = STATUS_ORDER.indexOf(this.order.status);
    const stepIdx = STATUS_ORDER.indexOf(stepKey);
    return stepIdx <= currentIdx;
  }

  getSubtotal(): number {
    return this.order?.items.reduce((s, i) => s + i.subTotal, 0) ?? 0;
  }
}
