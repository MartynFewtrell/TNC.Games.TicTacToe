Test Execution Prompt (test.execute.prompt.md)

Purpose
- Implement the Test Implementation Plan to build a reliable unit/functional and integration test suite.
- Work iteratively

How to use
- Open the latest plan in docs/plans/tests (plan-tests-*.md).
- Execute items top-to-bottom, one Work Item and Task at a time.

Process
1) Pick next Work Item
   - Read its requirement(s), class under test, and tasks.
2) Scaffold
   - Create/extend test files under tests/unit or tests/integration.
   - Add fixtures/fakes as needed (in-memory hosts for integration).
3) Implement
   - Write the test (Arrange-Act-Assert), run dotnet test to see it run.
   - Make minimal product changes if spec-compliant; re-run until green.
4) Update plan
   - Mark completed steps and list added files.
5) Repeat
   - Proceed to the next Work Item.

Guardrails
- One focused change at a time; ask clarification questions when needed.
- Tests must be deterministic, fast, and hermetic.
- Use .NET 8, xUnit; integration using .Net Aspire.
- No Thread.Sleep.
- Include positive, negative, edge, and regression cases.
- Include all imports/dependencies; build and run tests frequently.

Required outputs per assistant step
- Update the plan document to reflect progress.
- End with:
  "Step X Complete. Here's what I did and why: <brief>"
  "User instructions: Please do the following <commands/steps>"

Compact examples

Integration (Aspire / .NET style)
```csharp
[DataTestMethod]
[DataRow(2.0, 3.0, 5.0)]
[DataRow(5.0, 5.0, 10.0)]
public async Task AddTwoNumbersReturnsValidResult(double a, double b, double expected)
{
    // Arrange (normally in test setup)
    var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Sample_Aspire_Test_AppHost>();
    appHost.Services.ConfigureHttpClientDefaults(b => b.AddStandardResilienceHandler());
    var app = await appHost.BuildAsync();
    var rns = app.Services.GetRequiredService<ResourceNotificationService>();
    await app.StartAsync();
    var httpClient = app.CreateHttpClient("sample-api");
    await rns.WaitForResourceAsync("sample-api", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));

    // Act
    var request = new Request("add", (decimal)a, (decimal)b);
    var response = await httpClient.PostAsJsonAsync("/calculator", request);
    var result = await response.Content.ReadFromJsonAsync<Response>();

    // Assert
    Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    Assert.IsNotNull(result);
    Assert.AreEqual((decimal)expected, result!.Result);
}
```

Unit (class instantiation + fake dependency)
```csharp
public interface IDiscountPolicy { decimal Apply(decimal amount); }
public sealed class OrderCalculator(IDiscountPolicy policy) { public decimal Total(decimal a) => policy.Apply(a); }

public sealed class FakeDiscountPolicy : IDiscountPolicy { public decimal Apply(decimal a) => a * 0.90m; }

public class OrderCalculatorTests
{
    [Fact]
    public void Total_AppliesDiscount_ReturnsExpected()
    {
        var sut = new OrderCalculator(new FakeDiscountPolicy());
        var total = sut.Total(100m);
        Assert.Equal(90m, total);
    }
}
```

Notes
- In xUnit, use [Theory]/[InlineData] instead of [DataTestMethod]/[DataRow] if preferred.
- For this project, prefer Aspire for API tests.
- Deserialize responses with ReadFromJsonAsync<T>().
