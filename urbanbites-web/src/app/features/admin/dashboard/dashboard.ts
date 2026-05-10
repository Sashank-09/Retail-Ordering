import { CommonModule, DatePipe, CurrencyPipe } from "@angular/common";
import { Component, OnInit, inject } from "@angular/core";
import { RouterLink, Router } from "@angular/router";
import { DashboardStats } from "../../../core/models/admin.models";
import { AdminService } from "../../../core/services/admin.service";
import { AuthService } from "../../../core/services/auth.service";


@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, DatePipe],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css'],
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  isLoading = true;
  error = '';

  private authService = inject(AuthService);
  private router = inject(Router);
  private adminService = inject(AdminService);

  ngOnInit(): void {
    if (this.authService.isOwner() && !this.authService.isAdmin()) {
      this.router.navigate(['/admin/orders']);
      return;
    }
    this.loadStats();
  }

  loadStats(): void {
    this.isLoading = true;
    this.error = '';

    this.adminService.getDashboardStats().subscribe({
      next: (data) => {
        this.stats = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Failed to load dashboard stats:', err);
        this.error = 'Failed to load dashboard data.';
        this.isLoading = false;
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
}