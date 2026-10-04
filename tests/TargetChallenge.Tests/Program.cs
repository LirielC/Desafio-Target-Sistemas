using TargetChallenge.Models;
using TargetChallenge.Services;

var tests = new (string Name, Action Run)[]
{
    ("Venda abaixo de 100 não gera comissão", () => Equal(0m, new CommissionService().CalculateForSale(99.99m))),
    ("Venda de 100 gera 1%", () => Equal(1m, new CommissionService().CalculateForSale(100m))),
    ("Venda abaixo de 500 gera 1%", () => Equal(5m, new CommissionService().CalculateForSale(499.99m))),
    ("Venda de 500 gera 5%", () => Equal(25m, new CommissionService().CalculateForSale(500m))),
    ("Entrada aumenta estoque", TestEntry),
    ("Saída reduz estoque", TestExit),
    ("Saída sem saldo é rejeitada", TestInsufficientStock),
    ("Movimentações recebem IDs únicos", TestUniqueIds),
    ("Juros vencidos são calculados por dia", TestInterest),
    ("Título não vencido não gera juros", TestNoInterest)
};

var failures = 0;
foreach (var test in tests)
{
    try
    {
        test.Run();
        Console.WriteLine($"[OK] {test.Name}");
    }
    catch (Exception exception)
    {
        failures++;
        Console.WriteLine($"[FALHOU] {test.Name}: {exception.Message}");
    }
}

Console.WriteLine($"\n{tests.Length - failures}/{tests.Length} testes passaram.");
return failures == 0 ? 0 : 1;

static InventoryService NewInventory(int quantity = 10) =>
    new(new[] { new Product { Code = 101, Description = "Caneta", QuantityInStock = quantity } });

static void TestEntry()
{
    var result = NewInventory().Move(101, MovementType.Entry, 5, "Compra");
    Equal(15, result.FinalQuantity);
}

static void TestExit()
{
    var result = NewInventory().Move(101, MovementType.Exit, 4, "Venda");
    Equal(6, result.FinalQuantity);
}

static void TestInsufficientStock()
{
    var inventory = NewInventory();
    Throws<InvalidOperationException>(() => inventory.Move(101, MovementType.Exit, 11, "Venda"));
}

static void TestUniqueIds()
{
    var inventory = NewInventory();
    var first = inventory.Move(101, MovementType.Entry, 1, "Compra 1");
    var second = inventory.Move(101, MovementType.Entry, 1, "Compra 2");
    if (first.Movement.Id == second.Movement.Id)
        throw new Exception("Os IDs deveriam ser diferentes.");
}

static void TestInterest()
{
    var result = new InterestService().Calculate(1000m, new DateOnly(2026, 1, 1), new DateOnly(2026, 1, 3));
    Equal(2, result.DaysLate);
    Equal(50m, result.InterestAmount);
    Equal(1050m, result.UpdatedAmount);
}

static void TestNoInterest()
{
    var result = new InterestService().Calculate(1000m, new DateOnly(2026, 1, 3), new DateOnly(2026, 1, 1));
    Equal(0, result.DaysLate);
    Equal(0m, result.InterestAmount);
}

static void Equal<T>(T expected, T actual) where T : IEquatable<T>
{
    if (!expected.Equals(actual))
        throw new Exception($"Esperado: {expected}; obtido: {actual}.");
}

static void Throws<TException>(Action action) where TException : Exception
{
    try
    {
        action();
    }
    catch (TException)
    {
        return;
    }

    throw new Exception($"Era esperada uma exceção {typeof(TException).Name}.");
}
