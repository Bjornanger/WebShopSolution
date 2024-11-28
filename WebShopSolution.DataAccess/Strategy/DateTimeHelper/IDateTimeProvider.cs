namespace WebShopSolution.DataAccess.Strategy.DateTimeHelper;

public interface IDateTimeProvider
{
    DateTime Now { get; }
}