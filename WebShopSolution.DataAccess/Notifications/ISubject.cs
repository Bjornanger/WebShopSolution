using WebShopSolution.Shared.Interfaces;

namespace WebShopSolution.DataAccess.Notifications;

public interface ISubject<TEntity> where TEntity : IEntity
{
    void Attach(INotificationObserver<TEntity> observer);//Lägger på en observer
    void Detach(INotificationObserver<TEntity> observer);//Tar bort en observer
    void Notify(TEntity entity);//Notifierar alla observer om en ny produkt är tillagd
}