import { Component, computed, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './navbar.html',
  styleUrl: './navbar.css',
})
export class NavbarComponent {
  private authService = inject(AuthService);
  private cartService = inject(CartService);

  menuOpen = false;
  isLoggedIn = this.authService.isLoggedIn;
  isAdmin = this.authService.isAdmin;
  isOwner = this.authService.isOwner;
  cartCount      = this.cartService.itemCount;
  guestCartCount = this.cartService.guestCount;

  initials = computed(() => {
    const name = this.authService.userFullName();
    return name
      ? name
          .split(' ')
          .map((n) => n[0])
          .join('')
          .toUpperCase()
          .slice(0, 2)
      : 'U';
  });

  toggleMenu(): void {
    this.menuOpen = !this.menuOpen;
  }
  closeMenu(): void {
    this.menuOpen = false;
  }
  logout(): void {
    this.menuOpen = false;
    this.authService.logout();
  }
}
