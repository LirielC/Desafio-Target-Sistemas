using TargetChallenge.Models;

namespace TargetChallenge.Services;

public sealed class InventoryService
{
    private readonly Dictionary<int, Product> _products;
    private readonly List<StockMovement> _movements = [];

    public InventoryService(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        _products = products.ToDictionary(product => product.Code);
    }

    public IReadOnlyCollection<Product> Products => _products.Values;
    public IReadOnlyList<StockMovement> Movements => _movements.AsReadOnly();

    public StockMovementResult Move(int productCode, MovementType type, int quantity, string description)
    {
        if (!_products.TryGetValue(productCode, out var product))
            throw new KeyNotFoundException($"Produto de código {productCode} não encontrado.");

        if (!Enum.IsDefined(type))
            throw new ArgumentOutOfRangeException(nameof(type), "Tipo de movimentação inválido.");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "A quantidade deve ser maior que zero.");

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição da movimentação é obrigatória.", nameof(description));

        var finalQuantity = type == MovementType.Entry
            ? checked(product.QuantityInStock + quantity)
            : product.QuantityInStock - quantity;

        if (finalQuantity < 0)
            throw new InvalidOperationException(
                $"Estoque insuficiente. Saldo atual: {product.QuantityInStock}; saída solicitada: {quantity}.");

        var movement = new StockMovement(
            Guid.NewGuid(),
            productCode,
            type,
            quantity,
            description.Trim(),
            DateTimeOffset.UtcNow);

        product.QuantityInStock = finalQuantity;
        _movements.Add(movement);

        return new StockMovementResult(movement, finalQuantity);
    }
}
