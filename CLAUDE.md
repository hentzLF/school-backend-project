# Claude Code Instructions

## Before modifying code

Before any codebase modification (creating, editing, deleting), always start with a structured summary:

- **What we're doing:** (short description of the action)
- **Why we're doing it:** (reasoning)
- **What files we are affecting:** (list of files)
- **Why those files:** (explanation)
- **What this changes in our codebase:** (impact description)

### Functions & Methods

- **One function = one job.** If you need to describe what a function does using the word "and", it should be two functions.
- **The name IS the documentation.** A function's name should fully describe what it does. If the name is accurate, comments are unnecessary.
- **Extract, don't nest.** If a function contains a block of logic that does a subtask, extract it into a separate function with a descriptive name. The parent function should read like a high-level summary.

Example — bad:
```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
{
    if (request.Items == null || !request.Items.Any())
        return BadRequest("Order must contain items");
    if (request.Items.Any(i => i.Quantity <= 0))
        return BadRequest("Invalid quantity");

    var order = new Order { CustomerId = request.CustomerId };
    foreach (var item in request.Items)
    {
        var product = await _context.Products.FindAsync(item.ProductId);
        order.Lines.Add(new OrderLine { Product = product, Quantity = item.Quantity });
        order.Total += product.Price * item.Quantity;
    }

    if (await _context.Customers.AnyAsync(c => c.Id == request.CustomerId && c.IsPremium))
        order.Total *= 0.9m;

    _context.Orders.Add(order);
    await _context.SaveChangesAsync();
    await _emailService.SendAsync(order.CustomerId, "Order confirmed", $"Order {order.Id}");

    return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
}
```

Example — good:
```csharp
[HttpPost]
public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
{
    var validationResult = ValidateOrderRequest(request);
    if (validationResult is not null)
        return validationResult;

    var order = await BuildOrder(request);
    await ApplyPremiumDiscount(order);
    await SaveOrder(order);
    await SendOrderConfirmation(order);

    return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
}
```

The controller action reads like a summary. Each extracted method is self-explanatory, testable, and reusable.

## Git Conventions

- Use conventional commits: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`
- Keep subject lines under 72 characters
- Write commit messages in English, imperative mood ("Add endpoint", not "Added endpoint")
- Always run `dotnet build` before committing to verify compilation
- One logical change per commit — don't bundle unrelated changes