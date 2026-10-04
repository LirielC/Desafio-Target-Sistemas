using System.Text.Json.Serialization;

namespace TargetChallenge.Models;

public sealed record Sale(
    [property: JsonPropertyName("vendedor")] string Seller,
    [property: JsonPropertyName("valor")] decimal Amount);

public sealed class SalesData
{
    [JsonPropertyName("vendas")]
    public List<Sale> Sales { get; init; } = [];
}

public sealed record SellerCommission(string Seller, decimal SalesTotal, decimal CommissionTotal);
