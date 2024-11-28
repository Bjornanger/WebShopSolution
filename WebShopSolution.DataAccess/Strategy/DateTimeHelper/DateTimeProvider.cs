namespace WebShopSolution.DataAccess.Strategy.DateTimeHelper;

public class DateTimeProvider : IDateTimeProvider
{
    public virtual DateTime Now => DateTime.Now;
}