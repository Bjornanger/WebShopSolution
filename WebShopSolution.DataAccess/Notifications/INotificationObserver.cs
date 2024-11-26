using WebShopSolution.DataAccess.Entities;
using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Notifications
{
    // Gränssnitt för notifieringsobservatörer enligt Observer Pattern
    public interface INotificationObserver<TEntity> where TEntity : IEntity
    {

        void Update(TEntity entity); //Metod som kallas när något läggs till i databasen
    }
}
