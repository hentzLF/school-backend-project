# Claude Code Instructions

## Before modifying code

Before any codebase modification (creating, editing, deleting), always start with a structured summary:

- **What we're doing:** (short description of the action)
- **Why we're doing it:** (reasoning)
- **What files we are affecting:** (list of files)
- **Why those files:** (explanation)
- **What this changes in our codebase:** (impact description)

## Code Style

Write code that is easy to read, understand, and maintain. Follow these principles:

- **KISS (Keep It Simple, Stupid):** Choose the simplest solution that works. Avoid clever tricks, unnecessary abstractions, and over-engineering. If a junior developer can't understand the code in 30 seconds, it's too complex.
- **YAGNI (You Aren't Gonna Need It):** Don't build features, abstractions, or flexibility for hypothetical future needs. Solve today's problem today.
- **Readability over cleverness:** Code is read far more than it is written. Prefer explicit over implicit, verbose over terse, boring over clever.
- **Flat over nested:** Avoid deep nesting. Use early returns, guard clauses, and extract logic into well-named helper functions.

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

## Error Handling

- Never silently catch and ignore exceptions. If you catch, log or rethrow.
- Use ProblemDetails for API error responses (RFC 9457). Do not return raw exception messages to the client.
- Validate input at the controller/endpoint level. Do not rely on deep layers to catch bad data.
- Use guard clauses and early returns instead of deeply nested try-catch blocks.

## Testing

- When adding or modifying a feature, write or update unit tests in the corresponding test project.
- Use xUnit with FluentAssertions.
- Test names follow the pattern: `MethodName_Scenario_ExpectedResult`
- Keep tests focused: one assertion per test where practical.
- Do not use in-memory database for EF Core tests unless explicitly asked. Prefer Testcontainers or a real test database.