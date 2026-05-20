## Why

The Client MVC `BookingsController.Pay()` action is a stub — it transitions the booking directly to `Confirmed` status without processing any payment. The existing `IClientPaymentService` with `PayAsync()` and `GetHistoryAsync()` is never called from MVC, so the checkout flow has no payment method selection, no fee calculation, no receipt, and no payment history. This breaks the booking lifecycle and leaves domain logic unproven in the MVC layer.

## What Changes

- Replace the stubbed `BookingsController.Pay()` POST action with a proper two-step checkout flow: GET shows a checkout form, POST processes payment via `IClientPaymentService.PayAsync()`
- Add a checkout form view with payment method selection (Card, BankTransfer, Cash), fee breakdown (amount, 5% platform fee, total), and confirmation
- Add a payment receipt view shown after successful payment, displaying `PaymentReceiptDto` data
- Add a payment history page where clients can view all their past payments via `IClientPaymentService.GetHistoryAsync()`
- Add corresponding ViewModels for checkout, receipt, and history views
- Add resx translation keys for all new UI strings (EN + ET)

## Capabilities

### New Capabilities

- `client-checkout`: Checkout form with payment method selection, fee breakdown, and payment processing via `IClientPaymentService.PayAsync()`
- `client-payment-receipt`: Post-payment receipt view displaying payment confirmation details
- `client-payment-history`: Payment history listing page using `IClientPaymentService.GetHistoryAsync()`

### Modified Capabilities

- `client-booking-management-mvc`: The `Pay` action changes from a blind status update to a redirect into the checkout flow

## Impact

- **Controllers**: `BookingsController` — modify `Pay()`, add `Checkout()` GET action; new `PaymentsController` in Client area for receipt + history
- **Views**: 3 new views (Checkout form, Receipt, History index) + update Bookings/Details to link to checkout
- **ViewModels**: 3 new ViewModels (CheckoutViewModel, ReceiptViewModel, PaymentHistoryViewModel)
- **Mappers**: New `PaymentViewModelMapper` extension methods for Client area
- **Resources**: New resx keys in SharedResource.resx and SharedResource.et.resx
- **Dependencies**: `IClientPaymentService` already exists — no new BLL/DAL changes needed
