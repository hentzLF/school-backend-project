## 1. ViewModels

- [ ] 1.1 Create `CheckoutViewModel` (booking summary, fee breakdown, payment method options)
- [ ] 1.2 Create `CheckoutSubmitViewModel` (BookingId + selected PaymentMethod for POST binding)
- [ ] 1.3 Create `ReceiptViewModel` (payment receipt display data)
- [ ] 1.4 Create `PaymentHistoryViewModel` and `PaymentHistoryItemViewModel` (payment history listing)

## 2. Mappers

- [ ] 2.1 Create Client `PaymentViewModelMapper` with extension methods for ReceiptViewModel and PaymentHistoryItemViewModel mapping

## 3. Controllers

- [ ] 3.1 Add `Checkout` GET action to `BookingsController` — load booking, validate ownership + AwaitingPayment status, build CheckoutViewModel
- [ ] 3.2 Replace stubbed `Pay` POST action in `BookingsController` with `Checkout` POST — call `IClientPaymentService.PayAsync()`, redirect to receipt on success, handle BusinessRuleException
- [ ] 3.3 Create `PaymentsController` in Client area with `Receipt(Guid id)` GET action — load payment, validate client ownership, map to ReceiptViewModel
- [ ] 3.4 Add `Index` GET action to `PaymentsController` — call `IClientPaymentService.GetHistoryAsync()`, map to PaymentHistoryViewModel

## 4. Views

- [ ] 4.1 Create `Bookings/Checkout.cshtml` — payment method radio buttons (Card, BankTransfer, Cash), fee breakdown, confirm button
- [ ] 4.2 Create `Payments/Receipt.cshtml` — payment confirmation details with link back to booking
- [ ] 4.3 Create `Payments/Index.cshtml` — payment history table with empty-state message
- [ ] 4.4 Update `Bookings/Details.cshtml` — change checkout card button from direct POST to link to `/Client/Bookings/Checkout/{id}`

## 5. Navigation and Localization

- [ ] 5.1 Add "Payments" link to `_ClientLayout.cshtml` navigation
- [ ] 5.2 Add new resx keys to `SharedResource.resx` (EN) and `SharedResource.et.resx` (ET) for checkout, receipt, and history views

## 6. Tests

- [ ] 6.1 Add unit tests for `BookingsController.Checkout` GET/POST actions (ownership, status validation, PayAsync integration)
- [ ] 6.2 Add unit tests for `PaymentsController.Receipt` and `Index` actions
