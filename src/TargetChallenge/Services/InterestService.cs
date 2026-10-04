using TargetChallenge.Models;

namespace TargetChallenge.Services;

public sealed class InterestService
{
    public const decimal DailyRate = 0.025m;

    public InterestResult Calculate(decimal amount, DateOnly dueDate, DateOnly calculationDate)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "O valor não pode ser negativo.");

        var daysLate = Math.Max(0, calculationDate.DayNumber - dueDate.DayNumber);
        var interest = decimal.Round(amount * DailyRate * daysLate, 2, MidpointRounding.AwayFromZero);
        var updatedAmount = decimal.Round(amount + interest, 2, MidpointRounding.AwayFromZero);

        return new InterestResult(amount, dueDate, calculationDate, daysLate, interest, updatedAmount);
    }
}
