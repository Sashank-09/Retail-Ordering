import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';
import { environment } from '../../../../environments/environment';

declare const google: any;

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  returnUrl = '/';
  showPassword = false;

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
      return;
    }
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    this.initGoogleSignIn();
  }

  private initGoogleSignIn(): void {
    const interval = setInterval(() => {
      if (typeof google !== 'undefined' && google.accounts) {
        clearInterval(interval);

        google.accounts.id.initialize({
          client_id: environment.googleClientId,
          callback: (response: any) => this.handleGoogleResponse(response),
        });

        google.accounts.id.renderButton(document.getElementById('google-btn'), {
          theme: 'outline',
          size: 'large',
          width: 360,
          text: 'signin_with',
          shape: 'rectangular',
        });
      }
    }, 100);

    setTimeout(() => clearInterval(interval), 5000);
  }

  private handleGoogleResponse(response: any): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.authService.googleSignIn({ googleToken: response.credential }).subscribe({
      next: () => {
        this.cartService.loadCart().subscribe();
        this.router.navigate([this.returnUrl]);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Google sign-in failed.';
      },
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';
    this.authService.login(this.loginForm.value).subscribe({
      next: () => {
        this.cartService.loadCart().subscribe();
        this.router.navigate([this.returnUrl]);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Invalid email or password.';
      },
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  get email() {
    return this.loginForm.get('email')!;
  }
  get password() {
    return this.loginForm.get('password')!;
  }
}
