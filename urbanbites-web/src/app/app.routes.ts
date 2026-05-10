import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { adminGuard } from './core/guards/admin.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/home/home').then(m => m.HomeComponent)
  },
  {
    path: 'products',
    loadComponent: () =>
      import('./features/products/product-list/product-list')
        .then(m => m.ProductListComponent)
  },
  {
    path: 'products/:id',
    loadComponent: () =>
      import('./features/products/product-detail/product-detail')
        .then(m => m.ProductDetailComponent)
  },
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./features/auth/login/login')
            .then(m => m.LoginComponent)
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./features/auth/register/register')
            .then(m => m.RegisterComponent)
      },
      {
        path: 'forgot-password',
        loadComponent: () =>
          import('./features/auth/forgot-password/forgot-password')
            .then(m => m.ForgotPasswordComponent)
      }
    ]
  },
  {
    path: 'cart',
    loadComponent: () =>
      import('./features/cart/cart').then(m => m.CartComponent)
  },
  {
    path: 'checkout',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/cart/checkout/checkout')
        .then(m => m.CheckoutComponent)
  },
  {
    path: 'my-bookings',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/orders/order-list/order-list')
        .then(m => m.OrderListComponent)
  },
  {
    path: 'my-bookings/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/orders/order-detail/order-detail')
        .then(m => m.OrderDetailComponent)
  },
  {
    path: 'offers',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/offers/offers').then(m => m.OffersComponent)
  },
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/profile/profile')
        .then(m => m.ProfileComponent)
  },
  {
    path: 'admin',
    canActivate: [authGuard, adminGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./features/admin/dashboard/dashboard')
            .then(m => m.DashboardComponent)
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./features/admin/manage-products/manage-products')
            .then(m => m.ManageProductsComponent)
      },
      {
        path: 'orders',
        loadComponent: () =>
          import('./features/admin/manage-orders/manage-orders')
            .then(m => m.ManageOrdersComponent)
      },
      {
        path: 'promotions',
        loadComponent: () =>
          import('./features/admin/manage-promotions/manage-promotions')
            .then(m => m.ManagePromotionsComponent)
      },
      {
        path: 'coupons',
        loadComponent: () =>
          import('./features/admin/manage-coupons/manage-coupons')
            .then(m => m.ManageCouponsComponent)
      },
      {
        path: 'users',
        loadComponent: () =>
          import('./features/admin/manage-users/manage-users')
            .then(m => m.ManageUsersComponent)
      }
    ]
  },
  { path: '**', redirectTo: '' }
];