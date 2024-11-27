using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Strategy;

public class DiscountStrategyFactory
{
    public virtual IDiscountStrategy GetDiscountStrategy(DateTime currentDate)
    {
        if (IsBlackFriday((currentDate)))
        {
            return new BlackFridayDiscountStrategy(50.0);
        }

        return new NoDiscountStrategy();
    }

    //Här kan man lägga in tex Julrea eller sommarrea.
    private bool IsBlackFriday(DateTime currentDate)
    {
        return currentDate.Month == 11 && currentDate.Day == 29;//Detta är bara ett exempel då detta datum skiftar per år.
    }
   

}