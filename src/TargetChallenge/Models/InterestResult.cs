namespace TargetChallenge.Models;

public sealed record InterestResult(
    decimal OriginalAmount,
    DateOnly DueDate,
    DateOnly CalculationDate,
    int DaysLate,
    decimal InterestAmount,
    decimal UpdatedAmount);
