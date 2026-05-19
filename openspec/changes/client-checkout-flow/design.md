## Context

The Client MVC booking flow currently stubs the payment step — `BookingsController.Pay()` directly transitions the booking to `Confirmed` without calling `IClientPaymentService`. The BLL layer is fully implemented: `ClientPaymentService.PayAsync()` handles validation, fee calculation (5%), payment entity creation, and booking status transition. `GetHistoryAsync()` returns payment history. The only missing piece is the MVC presentation layer.

The current Bookings/Details view already shows a checkout card with fee breakdown when `CanPay` is true, but submits directly to the stubbed `Pay()` POST action without payment method selection.

## Goals / Non-Goals

**Goals:**
- Replace the stubbed `Pay()` with a proper checkout flow that calls `IClientPaymentService.PayAsync()`
- Add payment method selection UI (Card, BankTransfer, Cash)
- Show a payment receipt after successful payment
- Provide a payment history listing page
- Follow existing codebase patterns (ViewModels, Mappers, Localizer, area routing)

**Non-Goals:**
- Real payment gateway integration (Stripe, etc.) — the BLL already simulates payment processing
- Payment editing or refund initiation from the client side (admin-only)
- Email/notification on payment completion
- Pagination on payment history (keep it simple for assignment scope)

## Decisions

### 1. Checkout as a separate page vs. inline in booking details

**Decision:** Dedicated checkout page at `Bookings/Checkout/{id}`.

**Rationale:** The current Details view already has the fee breakdown card. Adding a payment method dropdown directly there would work, but a separate checkout page provides clearer UX separation (browse → decide → pay) and follows e-commerce conventions. The Details view will link to checkout with a "Proceed to Checkout" button instead of the current direct "ConfirmAndPay" submit.

**Alternative considered:** Keeping payment inline in Details — rejected because it mixes informational and transactional concerns.

### 2. Receipt and history in BookingsController vs. separate PaymentsController

**Decision:** New `PaymentsController` in the Client area for receipt and history. Checkout stays in `BookingsController` since it's part of the booking lifecycle.

**Rationale:** Follows single-responsibility — booking actions stay in BookingsController, payment-specific views (receipt, history) get their own controller. This also gives a clean `/Client/Payments` route for history and `/Client/Payments/Receipt/{id}` for receipts.

**Alternative considered:** All in BookingsController — rejected because payment history spans across bookings.

### 3. Checkout GET + POST pattern

**Decision:** `Checkout(Guid id)` GET renders the form, `Checkout(CheckoutSubmitViewModel model)` POST processes payment.

**Rationale:** Standard ASP.NET MVC PRG (Post-Redirect-Get) pattern. GET loads the booking, validates ownership and status, builds the CheckoutViewModel with fee breakdown. POST validates the selected payment method, calls `IClientPaymentService.PayAsync()`, and redirects to receipt.

### 4. Receipt lookup strategy

**Decision:** After `PayAsync()` returns a `PaymentReceiptDto`, redirect to `Payments/Receipt/{paymentId}`. The receipt page loads the payment from the database via `IPaymentService`.

**Rationale:** Using TempData to pass receipt data between redirect would work but is fragile (lost on refresh). Fetching from DB on the receipt page ensures the receipt is always accessible and bookmarkable.

**Note:** `IClientPaymentService` does not have a `GetByIdAsync` — we will use `IPaymentService.GetByIdAsync()` which already exists in the admin payment flow, adding an ownership check in the controller.

### 5. ViewModels

Three new ViewModels:
- `CheckoutViewModel` — booking summary, fee breakdown, payment method options (for GET)
- `CheckoutSubmitViewModel` — bookingId + selected PaymentMethod (for POST binding)
- `PaymentHistoryViewModel` — list of `PaymentHistoryItemDto` mapped to display items

Receipt will use `PaymentReceiptDto` directly via a mapper to a `ReceiptViewModel`.

## Risks / Trade-offs

- **[Risk] `IPaymentService.GetByIdAsync()` may not filter by client ownership** → Mitigation: Add explicit ownership check in `PaymentsController.Receipt()` comparing payment's booking client profile to authenticated user.
- **[Risk] Race condition on double-submit of checkout form** → Mitigation: `IClientPaymentService.PayAsync()` already validates booking status is `AwaitingPayment` — second submit will throw `BusinessRuleException` which the controller catches and redirects with error.
- **[Trade-off] No pagination on payment history** → Acceptable for assignment scope. If needed later, `GetHistoryAsync` can be extended with pagination parameters.
