using TargetChallenge.Models;

namespace TargetChallenge.Services;

public sealed class CommissionService
{
    public decimal CalculateForSale(decimal saleAmount)
    {
        if (saleAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(saleAmount), "O valor da venda não pode ser negativo.");

        var rate = saleAmount switch
        {
            < 100m => 0m,
            < 500m => 0.01m,
            _ => 0.05m
        };

        return decimal.Round(saleAmount * rate, 2, MidpointRounding.AwayFromZero);
    }

    public IReadOnlyList<SellerCommission> CalculateBySeller(IEnumerable<Sale> sales)
    {
        ArgumentNullException.ThrowIfNull(sales);

        return sales
            .GroupBy(sale => sale.Seller, StringComparer.OrdinalIgnoreCase)
            .Select(group => new SellerCommission(
                group.First().Seller,
                group.Sum(sale => sale.Amount),
                group.Sum(sale => CalculateForSale(sale.Amount))))
            .OrderBy(result => result.Seller, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }
}
