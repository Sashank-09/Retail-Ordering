import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth.service';
import { CartService } from '../../../core/services/cart.service';

function passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
  const password        = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  if (password && confirmPassword && password.value !== confirmPassword.value) {
    return { passwordMismatch: true };
  }
  return null;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CommonModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent implements OnInit {

  private fb          = inject(FormBuilder);
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private router      = inject(Router);

  registerForm: FormGroup;
  isLoading     = false;
  errorMessage  = '';
  showPassword  = false;
  showConfirm   = false;

  constructor() {
    this.registerForm = this.fb.group({
      fullName:        ['', [Validators.required, Validators.minLength(3)]],
      email:           ['', [Validators.required, Validators.email]],
      phone:           ['', [Validators.required, Validators.pattern(/^[0-9]{10}$/)]],
      password:        ['', [Validators.required, Validators.minLength(8),
                             Validators.pattern(/^(?=.*[A-Z])(?=.*[0-9])/)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordMatchValidator });
  }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/']);
    }
  }

  onSubmit(): void {
    if (this.registerForm.invalid) { this.registerForm.markAllAsTouched(); return; }
    this.isLoading   = true;
    this.errorMessage = '';

    const { fullName, email, phone, password } = this.registerForm.value;
    this.authService.register({ fullName, email, phone, password }).subscribe({
      next: () => {
        this.cartService.loadCart().subscribe();
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.isLoading   = false;
        this.errorMessage = err.error?.message || 'Registration failed. Please try again.';
      }
    });
  }

  get fullName()        { return this.registerForm.get('fullName')!; }
  get email()           { return this.registerForm.get('email')!; }
  get phone()           { return this.registerForm.get('phone')!; }
  get password()        { return this.registerForm.get('password')!; }
  get confirmPassword() { return this.registerForm.get('confirmPassword')!; }
}