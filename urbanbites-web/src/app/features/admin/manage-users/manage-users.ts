import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';

interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  roles: string[];
  highestRole: string;
  isLockedOut: boolean;
  createdAt?: string;
}

@Component({
  selector: 'app-manage-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-users.html',
  styleUrls: ['./manage-users.css']
})
export class ManageUsersComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  isLoading = true;
  error = '';
  searchQuery = '';
  selectedRole = 'All';
  roles = ['All', 'Owner', 'Admin', 'Customer'];

  // Whether the current logged-in user is an Owner
  isCurrentUserOwner = false;

  // Modal state
  showModal = false;
  modalUser: User | null = null;
  modalAction: 'role' | 'lock' | 'delete' | 'transfer' | null = null;
  isActionLoading = false;
  actionSuccess = '';

  private apiBase = 'http://localhost:5077/api/admin';

  constructor(private http: HttpClient, private auth: AuthService) {}

  ngOnInit() {
    this.isCurrentUserOwner = this.auth.isOwner();
    this.loadUsers();
  }

  private getHeaders() {
    return new HttpHeaders({ Authorization: `Bearer ${this.auth.getToken()}` });
  }

  loadUsers() {
    this.isLoading = true;
    this.error = '';
    this.http.get<User[]>(`${this.apiBase}/users`, { headers: this.getHeaders() })
      .subscribe({
        next: (data) => {
          this.users = data;
          this.applyFilters();
          this.isLoading = false;
        },
        error: () => {
          this.error = 'Failed to load users.';
          this.isLoading = false;
        }
      });
  }

  applyFilters() {
    let result = [...this.users];
    if (this.selectedRole !== 'All') {
      result = result.filter(u => u.highestRole === this.selectedRole);
    }
    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase();
      result = result.filter(u =>
        u.firstName?.toLowerCase().includes(q) ||
        u.lastName?.toLowerCase().includes(q) ||
        u.email.toLowerCase().includes(q)
      );
    }
    this.filteredUsers = result;
  }

  onSearch() { this.applyFilters(); }
  onRoleFilter(role: string) { this.selectedRole = role; this.applyFilters(); }

  openLockModal(user: User) {
    this.modalUser = user;
    this.modalAction = 'lock';
    this.showModal = true;
    this.actionSuccess = '';
  }

  openRoleModal(user: User) {
    this.modalUser = user;
    this.modalAction = 'role';
    this.showModal = true;
    this.actionSuccess = '';
  }

  openDeleteModal(user: User) {
    this.modalUser = user;
    this.modalAction = 'delete';
    this.showModal = true;
    this.actionSuccess = '';
  }

  openTransferModal(user: User) {
    this.modalUser = user;
    this.modalAction = 'transfer';
    this.showModal = true;
    this.actionSuccess = '';
  }

  closeModal() {
    this.showModal = false;
    this.modalUser = null;
    this.modalAction = null;
    this.actionSuccess = '';
  }

  confirmToggleLock() {
    if (!this.modalUser) return;
    this.isActionLoading = true;
    const endpoint = this.modalUser.isLockedOut
      ? `${this.apiBase}/users/${this.modalUser.id}/unlock`
      : `${this.apiBase}/users/${this.modalUser.id}/lock`;
    this.http.post(endpoint, {}, { headers: this.getHeaders() }).subscribe({
      next: () => {
        this.modalUser!.isLockedOut = !this.modalUser!.isLockedOut;
        this.isActionLoading = false;
        this.actionSuccess = `User ${this.modalUser!.isLockedOut ? 'locked' : 'unlocked'} successfully.`;
        setTimeout(() => this.closeModal(), 1500);
      },
      error: (err) => {
        this.isActionLoading = false;
        this.actionSuccess = err?.error?.message ?? 'Action failed.';
      }
    });
  }

  confirmToggleAdmin() {
    if (!this.modalUser) return;
    this.isActionLoading = true;
    const isAdmin = this.modalUser.roles.includes('Admin');
    const endpoint = isAdmin
      ? `${this.apiBase}/users/${this.modalUser.id}/remove-admin`
      : `${this.apiBase}/users/${this.modalUser.id}/make-admin`;
    this.http.post(endpoint, {}, { headers: this.getHeaders() }).subscribe({
      next: () => {
        if (isAdmin) {
          this.modalUser!.roles = this.modalUser!.roles.filter(r => r !== 'Admin');
          this.modalUser!.highestRole = 'Customer';
        } else {
          this.modalUser!.roles.push('Admin');
          this.modalUser!.highestRole = 'Admin';
        }
        this.isActionLoading = false;
        this.actionSuccess = `Role updated successfully.`;
        setTimeout(() => this.closeModal(), 1500);
      },
      error: (err) => {
        this.isActionLoading = false;
        this.actionSuccess = err?.error?.message ?? 'Action failed.';
      }
    });
  }

  confirmDeleteAccount() {
    if (!this.modalUser) return;
    this.isActionLoading = true;
    this.http.delete(`${this.apiBase}/users/${this.modalUser.id}`, { headers: this.getHeaders() }).subscribe({
      next: () => {
        this.users = this.users.filter(u => u.id !== this.modalUser!.id);
        this.applyFilters();
        this.isActionLoading = false;
        this.actionSuccess = 'Account deleted successfully.';
        setTimeout(() => this.closeModal(), 1500);
      },
      error: (err) => {
        this.isActionLoading = false;
        this.actionSuccess = err?.error?.message ?? 'Failed to delete account.';
      }
    });
  }

  confirmTransferOwnership() {
    if (!this.modalUser) return;
    this.isActionLoading = true;
    this.http.post(`${this.apiBase}/users/${this.modalUser.id}/transfer-ownership`, {}, { headers: this.getHeaders() }).subscribe({
      next: () => {
        this.isActionLoading = false;
        this.actionSuccess = 'Ownership transferred! You will be redirected.';
        setTimeout(() => {
          this.closeModal();
          // Refresh to reflect the new role
          this.auth.logout();
        }, 2000);
      },
      error: (err) => {
        this.isActionLoading = false;
        this.actionSuccess = err?.error?.message ?? 'Failed to transfer ownership.';
      }
    });
  }

  getInitials(user: User): string {
    return `${user.firstName?.[0] ?? ''}${user.lastName?.[0] ?? ''}`.toUpperCase();
  }

  getPrimaryRole(user: User): string {
    return user.highestRole || 'Customer';
  }

  // Can the current user perform role actions on this user?
  canManageRole(user: User): boolean {
    if (!this.isCurrentUserOwner) return false;
    // Cannot change Owner's role
    if (user.highestRole === 'Owner') return false;
    return true;
  }

  // Can the current user lock/unlock this user?
  canLockUser(user: User): boolean {
    // Owner users cannot be locked
    if (user.highestRole === 'Owner') return false;
    // Admins can only be locked by Owners
    if (user.highestRole === 'Admin' && !this.isCurrentUserOwner) return false;
    return true;
  }

  // Can the current user delete this user's account?
  canDeleteAccount(user: User): boolean {
    if (!this.isCurrentUserOwner) return false;
    if (user.highestRole === 'Owner') return false;
    return true;
  }

  // Can the current user transfer ownership to this user?
  canTransferOwnership(user: User): boolean {
    if (!this.isCurrentUserOwner) return false;
    if (user.highestRole === 'Owner') return false;
    return true;
  }
}