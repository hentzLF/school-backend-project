# Testing Requirements

## Minimum Test Coverage: 80%

Test Types (ALL required):
1. **Unit Tests** - Individual functions, utilities, services
2. **Integration Tests** - API endpoints, database operations
3. **E2E Tests** - Critical user flows

## Test-Driven Development

MANDATORY workflow:
1. Write test first (RED)
2. Run test - it should FAIL
3. Write minimal implementation (GREEN)
4. Run test - it should PASS
5. Refactor (IMPROVE)
6. Verify coverage (80%+)

## Troubleshooting Test Failures

1. Use **tdd-guide** agent
2. Check test isolation
3. Verify mocks are correct
4. Fix implementation, not tests (unless tests are wrong)

## Agent Support

- **tdd-guide** - Use PROACTIVELY for new features, enforces write-tests-first

## Test Structure (AAA Pattern)

Prefer Arrange-Act-Assert structure for tests:

```csharp
[Fact]
public async Task CalculateSimilarity_OrthogonalVectors_ReturnsZero()
{
    // Arrange
    var vector1 = new[] { 1.0, 0.0, 0.0 };
    var vector2 = new[] { 0.0, 1.0, 0.0 };

    // Act
    var similarity = CalculateCosineSimilarity(vector1, vector2);

    // Assert
    similarity.Should().Be(0);
}
```

### Test Naming

Use descriptive names following the pattern `MethodName_Scenario_ExpectedResult`:

```csharp
[Fact]
public async Task Search_NoMatchingMarkets_ReturnsEmptyList() { }

[Fact]
public void GetApiKey_KeyNotConfigured_ThrowsInvalidOperationException() { }

[Fact]
public async Task Search_RedisUnavailable_FallsBackToSubstringSearch() { }
```
