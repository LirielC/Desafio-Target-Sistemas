namespace TargetChallenge.Models;

public enum MovementType
{
    Entry = 1,
    Exit = 2
}

public sealed record StockMovement(
    Guid Id,
    int ProductCode,
    MovementType Type,
    int Quantity,
    string Description,
    DateTimeOffset CreatedAt);

public sealed record StockMovementResult(StockMovement Movement, int FinalQuantity);
