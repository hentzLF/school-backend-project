## 1. ViewModels and Mappers

- [ ] 1.1 Create `CheckoutViewModel` (booking summary, fee breakdown, payment method options)
- [ ] 1.2 Create `CheckoutSubmitViewModel` (BookingId + selected PaymentMethod for POST binding)
- [ ] 1.3 Create `ReceiptViewModel` (payment receipt display data)
- [ ] 1.4 Create `PaymentHistoryViewModel` and `PaymentHistoryItemViewModel` (payment history listing)
- [ ] 1.5 Create Client `PaymentViewModelMapper` with extension methods for ReceiptViewModel and PaymentHistoryItemViewModel mapping

> **GIT COMMIT:** `feat: add checkout and payment ViewModels and mapper`

## 2. Controllers

- [ ] 2.1 Add `Checkout` GET action to `BookingsController` — load booking, validate ownership + AwaitingPayment status, build CheckoutViewModel
- [ ] 2.2 Replace stubbed `Pay` POST action in `BookingsController` with `Checkout` POST — call `IClientPaymentService.PayAsync()`, redirect to receipt on success, handle BusinessRuleException
- [ ] 2.3 Create `PaymentsController` in Client area with `Receipt(Guid id)` GET action — load payment, validate client ownership, map to ReceiptViewModel
- [ ] 2.4 Add `Index` GET action to `PaymentsController` — call `IClientPaymentService.GetHistoryAsync()`, map to PaymentHistoryViewModel

> **GIT COMMIT:** `feat: implement checkout and payment controller actions`

## 3. Views

- [ ] 3.1 Create `Bookings/Checkout.cshtml` — payment method radio buttons (Card, BankTransfer, Cash), fee breakdown, confirm button
- [ ] 3.2 Create `Payments/Receipt.cshtml` — payment confirmation details with link back to booking
- [ ] 3.3 Create `Payments/Index.cshtml` — payment history table with empty-state message
- [ ] 3.4 Update `Bookings/Details.cshtml` — change checkout card button from direct POST to link to `/Client/Bookings/Checkout/{id}`

> **GIT COMMIT:** `feat: add checkout, receipt, and payment history views`

## 4. Navigation and Localization

- [ ] 4.1 Add "Payments" link to `_ClientLayout.cshtml` navigation
- [ ] 4.2 Add new resx keys to `SharedResource.resx` (EN) and `SharedResource.et.resx` (ET) for checkout, receipt, and history views

> **GIT COMMIT:** `feat: add payment navigation link and localization strings`

## 5. Tests

- [ ] 5.1 Add unit tests for `BookingsController.Checkout` GET/POST actions (ownership, status validation, PayAsync integration)
- [ ] 5.2 Add unit tests for `PaymentsController.Receipt` and `Index` actions

> **GIT COMMIT:** `test: add checkout and payment controller unit tests`
