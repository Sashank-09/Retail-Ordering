# 🍕 UrbanBites — Full Stack Food Ordering Platform

[![Angular](https://img.shields.io/badge/Frontend-Angular_20-dd1b16?logo=angular&style=flat-square)](https://angular.io/)
[![.NET](https://img.shields.io/badge/Backend-.NET_8_Core-512bd4?logo=dotnet&style=flat-square)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/Database-PostgreSQL_(Supabase)-336791?logo=postgresql&style=flat-square)](https://supabase.com/)
[![Vercel](https://img.shields.io/badge/Hosting-Vercel-000000?logo=vercel&style=flat-square)](https://vercel.com/)
[![Render](https://img.shields.io/badge/API_Hosting-Render-46E3B7?logo=render&logoColor=black&style=flat-square)](https://render.com/)

UrbanBites is a robust, modern, and fully-deployed e-commerce ecosystem engineered for food retail. It features a sleek, real-time dashboard, secure payment flow, multi-factor cloud communications, and an enterprise-ready architectural blueprint.

🔗 **Live Website:** [retail-ordering.vercel.app](https://retail-ordering.vercel.app)

---

## ✨ Key Features

### 🔐 Advanced Identity & Security
- **Google One-Tap SSO:** Quick seamless logins via Google Identity Platform.
- **Standard Auth:** Robust password policy enforcement via ASP.NET Identity core.
- **Secure Recovery:** Time-sensitive, hashed OTP codes delivered via automated emails for secure password resets.

### 🛒 Core E-Commerce Workflow
- **Dynamic Cart & Checkout:** State-managed persistent cart flow.
- **Coupon Engine:** Intelligent backend calculation of discount thresholds and validity periods.
- **Loyalty Tier Rewards:** Integrated points accumulation system allowing customers to earn "bites cashback" with every real purchase.

### 💳 Production Payments
- **Razorpay Gateway Integration:** Smooth transaction handling with client-side modal interface.
- **Backend Verification:** High-integrity server-to-server signature checksum checking to prevent tampering before persisting orders.

### 📬 Integrated Communications Engine
- **SendGrid Powered API:** Real-time triggered emails.
- **Dynamic Templates:** Cleanly designed automated HTML messages for:
  - Welcome & Onboarding
  - Password Reset OTP
  - Secure Invoice Receipts (includes Payment ID)
  - **Real-time Tracking Updates:** Instant notifications triggered directly from server hooks when order status changes (Preparing ➡️ Out for Delivery).

---

## 🏗 Tech Stack Architecture

### Backend (`/UrbanBite`)
- **Framework:** ASP.NET Core 8 (Clean Architecture principles)
- **Database:** PostgreSQL (Hosted via **Supabase**)
- **ORM:** Entity Framework Core with automated relational migrations
- **Authentication:** JWT Bearer token headers & ASP.NET Core Identity
- **Mailing:** SendGrid C# SDK
- **Payments:** Razorpay .NET v3.3.2 SDK

### Frontend (`/urbanbites-web`)
- **Framework:** Angular v20
- **Styling:** Pure CSS with modern glassmorphism, Flexbox grid, and dark-palette compliance.
- **Form Handling:** Strongly typed Angular Reactive Forms with custom validations.

---

## 🚀 Continuous Deployment Pipeline

The application relies on a fully synchronized CI/CD lifecycle:

1. **Database:** Permanently active on cloud **Supabase** engine.
2. **Backend:** Dockered and auto-scaling on **Render.com**, rebuilding seamlessly on every git push to main.
3. **Frontend:** High-speed distribution from **Vercel CDN**, directly communicating securely with the live production API environment.

---

## 🛠 Local Development Setup

### Prerequisites
- Node.js v20+ & Angular CLI (`npm install -g @angular/cli`)
- .NET 8 SDK
- PostgreSQL (or cloud instance)

### Step 1: Clone & Environment Configuration
```bash
git clone https://github.com/Sashank-09/Retail-Ordering.git
cd Retail-Ordering
```
Populate secret keys inside `UrbanBite/UrbanBites.API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": { "DefaultConnection": "Server=..." },
  "Razorpay": { "KeyId": "rzp_test_...", "KeySecret": "..." },
  "EmailSettings": { "SendGridApiKey": "SG.XXX", "SenderEmail": "..." }
}
```

### Step 2: Boot the Server
```bash
cd UrbanBite/UrbanBites.API
dotnet run
```

### Step 3: Boot the Frontend
```bash
cd ../../urbanbites-web
npm install
ng serve --open
```
The application will automatically wire up internally and launch!

---

*Built with ❤️ utilizing .NET Web API & Angular 20.*
