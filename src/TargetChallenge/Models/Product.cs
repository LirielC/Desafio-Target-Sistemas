using System.Text.Json.Serialization;

namespace TargetChallenge.Models;

public sealed class Product
{
    [JsonPropertyName("codigoProduto")]
    public int Code { get; init; }

    [JsonPropertyName("descricaoProduto")]
    public string Description { get; init; } = string.Empty;

    [JsonPropertyName("estoque")]
    public int QuantityInStock { get; set; }
}

public sealed class InventoryData
{
    [JsonPropertyName("estoque")]
    public List<Product> Products { get; init; } = [];
}
