import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AuthResponse,
  LoginRequest,
  RegisterRequest,
  ForgotPasswordRequest,
  VerifyOtpRequest,
  ResetPasswordRequest,
  GoogleSignInRequest,
  UserProfile,
  UpdateProfileRequest,
} from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;
  private readonly userUrl = `${environment.apiUrl}/user`;

  private _currentUser = signal<AuthResponse | null>(this.loadFromStorage());

  currentUser = this._currentUser.asReadonly();
  isLoggedIn = computed(() => !!this._currentUser());
  isAdmin = computed(() => {
    const role = this._currentUser()?.role;
    return role === 'Admin';
  });
  isOwner = computed(() => this._currentUser()?.role === 'Owner');
  userFullName = computed(() => this._currentUser()?.fullName ?? '');
  userEmail = computed(() => this._currentUser()?.email ?? '');

  constructor(
    private http: HttpClient,
    private router: Router,
  ) {}

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/register`, request)
      .pipe(tap((res) => this.saveSession(res)));
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/login`, request)
      .pipe(tap((res) => this.saveSession(res)));
  }

  googleSignIn(request: GoogleSignInRequest): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/google-signin`, request)
      .pipe(tap((res) => this.saveSession(res)));
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/forgot-password`, request);
  }

  verifyOtp(request: VerifyOtpRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/verify-otp`, request);
  }

  resetPassword(request: ResetPasswordRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(`${this.apiUrl}/reset-password`, request);
  }

  getProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.userUrl}/profile`);
  }

  updateProfile(request: UpdateProfileRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.userUrl}/profile`, request);
  }

  logout(): void {
    localStorage.removeItem('urbanbites_user');
    this._currentUser.set(null);
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    return this._currentUser()?.token ?? null;
  }

  private saveSession(response: AuthResponse): void {
    localStorage.setItem('urbanbites_user', JSON.stringify(response));
    this._currentUser.set(response);
  }

  private loadFromStorage(): AuthResponse | null {
    const stored = localStorage.getItem('urbanbites_user');
    if (!stored) return null;
    const user: AuthResponse = JSON.parse(stored);
    if (new Date(user.expiresAt) < new Date()) {
      localStorage.removeItem('urbanbites_user');
      return null;
    }
    return user;
  }
}
