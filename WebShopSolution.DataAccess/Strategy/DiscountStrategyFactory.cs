using WebShopSolution.DataAccess.Strategy.DateTimeHelper;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Strategy;

public class DiscountStrategyFactory
{
    private readonly IDateTimeProvider _dateTimeProvider;


    public DiscountStrategyFactory(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }


    public virtual IDiscountStrategy GetDiscountStrategy(DateTime currentDate)
    {
        if (IsBlackFriday())
        {
            return new BlackFridayDiscountStrategy(50.0);
        }

        return new NoDiscountStrategy();
    }

    //Här kan man lägga in tex Julrea eller sommarrea.
    private bool IsBlackFriday()
    {
        var currentDate = _dateTimeProvider.Now; //För att kunna manipulera testetstid måste detta finns här.
        return currentDate.Month == 11 && currentDate.Day == 29;//Detta är bara ett exempel då detta datum skiftar per år.
    }
   
}