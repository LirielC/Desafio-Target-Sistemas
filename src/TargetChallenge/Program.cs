using System.Globalization;
using TargetChallenge.Infrastructure;
using TargetChallenge.Models;
using TargetChallenge.Services;

var culture = CultureInfo.GetCultureInfo("pt-BR");
var dataDirectory = Path.Combine(AppContext.BaseDirectory, "Data");

try
{
    var salesData = JsonDataLoader.Load<SalesData>(Path.Combine(dataDirectory, "vendas.json"));
    var inventoryData = JsonDataLoader.Load<InventoryData>(Path.Combine(dataDirectory, "estoque.json"));
    var commissionService = new CommissionService();
    var inventoryService = new InventoryService(inventoryData.Products);
    var interestService = new InterestService();

    while (true)
    {
        Console.WriteLine("\n=== Desafio Target Sistemas ===");
        Console.WriteLine("1 - Calcular comissões");
        Console.WriteLine("2 - Movimentar estoque");
        Console.WriteLine("3 - Calcular juros");
        Console.WriteLine("0 - Sair");
        Console.Write("Escolha uma opção: ");

        switch (Console.ReadLine())
        {
            case "1":
                ShowCommissions(commissionService, salesData.Sales, culture);
                break;
            case "2":
                MoveStock(inventoryService);
                break;
            case "3":
                ShowInterest(interestService, culture);
                break;
            case "0":
                return;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
}
catch (Exception exception)
{
    Console.Error.WriteLine($"Erro ao iniciar a aplicação: {exception.Message}");
    Environment.ExitCode = 1;
}

static void ShowCommissions(CommissionService service, IEnumerable<Sale> sales, CultureInfo culture)
{
    Console.WriteLine("\nComissões por vendedor:");
    foreach (var result in service.CalculateBySeller(sales))
    {
        Console.WriteLine(
            $"- {result.Seller}: vendas {result.SalesTotal.ToString("C", culture)} | " +
            $"comissão {result.CommissionTotal.ToString("C", culture)}");
    }
}

static void MoveStock(InventoryService service)
{
    Console.WriteLine("\nProdutos:");
    foreach (var product in service.Products.OrderBy(product => product.Code))
        Console.WriteLine($"- {product.Code}: {product.Description} (saldo: {product.QuantityInStock})");

    var productCode = ReadInt("Código do produto: ");
    var typeNumber = ReadInt("Tipo (1 = entrada, 2 = saída): ");
    var quantity = ReadInt("Quantidade: ");
    Console.Write("Descrição da movimentação: ");
    var description = Console.ReadLine() ?? string.Empty;

    try
    {
        var result = service.Move(productCode, (MovementType)typeNumber, quantity, description);
        Console.WriteLine($"Movimentação {result.Movement.Id} registrada. Estoque final: {result.FinalQuantity}.");
    }
    catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or KeyNotFoundException)
    {
        Console.WriteLine($"Não foi possível movimentar o estoque: {exception.Message}");
    }
}

static void ShowInterest(InterestService service, CultureInfo culture)
{
    var amount = ReadDecimal("Valor original: R$ ", culture);
    var dueDate = ReadDate("Data de vencimento (dd/MM/aaaa): ", culture);
    var today = DateOnly.FromDateTime(DateTime.Today);

    var result = service.Calculate(amount, dueDate, today);
    Console.WriteLine($"Dias em atraso: {result.DaysLate}");
    Console.WriteLine($"Juros: {result.InterestAmount.ToString("C", culture)}");
    Console.WriteLine($"Valor atualizado: {result.UpdatedAmount.ToString("C", culture)}");
}

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        if (int.TryParse(Console.ReadLine(), out var value))
            return value;
        Console.WriteLine("Digite um número inteiro válido.");
    }
}

static decimal ReadDecimal(string prompt, CultureInfo culture)
{
    while (true)
    {
        Console.Write(prompt);
        if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, culture, out var value))
            return value;
        Console.WriteLine("Digite um valor válido, por exemplo 1250,50.");
    }
}

static DateOnly ReadDate(string prompt, CultureInfo culture)
{
    while (true)
    {
        Console.Write(prompt);
        if (DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", culture, DateTimeStyles.None, out var value))
            return value;
        Console.WriteLine("Digite uma data válida no formato dd/MM/aaaa.");
    }
}
