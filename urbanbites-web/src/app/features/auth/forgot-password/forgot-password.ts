import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  // 3 steps: email → otp → new password
  step = 1;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  verifiedEmail = '';

  emailForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  otpForm = this.fb.group({
    otpCode: [
      '',
      [
        Validators.required,
        Validators.minLength(6),
        Validators.maxLength(6),
        Validators.pattern(/^[0-9]+$/),
      ],
    ],
  });

  passwordForm = this.fb.group({
    newPassword: [
      '',
      [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9])/)],
    ],
    confirmPassword: ['', Validators.required],
  });

  // Step 1 — Send OTP
  sendOtp(): void {
    if (this.emailForm.invalid) {
      this.emailForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';
    this.authService.forgotPassword(this.emailForm.value as any).subscribe({
      next: () => {
        this.isLoading = false;
        this.verifiedEmail = this.emailForm.value.email!;
        this.step = 2;
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.message || 'Failed to send OTP.';
      },
    });
  }

  // Step 2 — Verify OTP
  verifyOtp(): void {
    if (this.otpForm.invalid) {
      this.otpForm.markAllAsTouched();
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';
    this.authService
      .verifyOtp({
        email: this.verifiedEmail,
        otpCode: this.otpForm.value.otpCode!,
      })
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.step = 3;
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Invalid OTP. Please try again.';
        },
      });
  }

  // Step 3 — Reset Password
  resetPassword(): void {
    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      return;
    }
    const { newPassword, confirmPassword } = this.passwordForm.value;
    if (newPassword !== confirmPassword) {
      this.errorMessage = 'Passwords do not match.';
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';
    this.authService
      .resetPassword({
        email: this.verifiedEmail,
        otpCode: this.otpForm.value.otpCode!,
        newPassword: newPassword!,
      })
      .subscribe({
        next: () => {
          this.isLoading = false;
          this.successMessage = 'Password reset successfully!';
          setTimeout(() => this.router.navigate(['/auth/login']), 2000);
        },
        error: (err) => {
          this.isLoading = false;
          this.errorMessage = err.error?.message || 'Failed to reset password.';
        },
      });
  }

  resendOtp(): void {
    this.authService.forgotPassword({ email: this.verifiedEmail }).subscribe({
      next: () => {
        this.successMessage = 'OTP resent successfully!';
      },
    });
  }

  get emailCtrl() {
    return this.emailForm.get('email')!;
  }
  get otpCtrl() {
    return this.otpForm.get('otpCode')!;
  }
  get newPasswordCtrl() {
    return this.passwordForm.get('newPassword')!;
  }
}
