using FakeItEasy;
using WebShopSolution.DataAccess.Data;
using WebShopSolution.DataAccess.Entities;
using WebShopSolution.DataAccess.Notifications;
using WebShopSolution.DataAccess.Repositories;
using WebShopSolution.DataAccess.Repositories.Customer;
using WebShopSolution.DataAccess.Repositories.Orders;
using WebShopSolution.DataAccess.Repositories.Products;
using WebShopSolution.DataAccess.UnitOfWork;

namespace WebShopTests
{
    public class UnitOfWorkTests
    {

        private readonly IRepositoryFactory _factory;
        private readonly MyDbContext _context;
        private readonly UnitOfWork _unitOfWork;

        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;



        //Testa Observer Pattern
        private readonly ProductSubject _productSubject;
        private readonly INotificationObserver<Product> _notificationObserver;


        public UnitOfWorkTests()
        {
            
            _factory = A.Fake<IRepositoryFactory>();
            _context = A.Fake<MyDbContext>();
            _productSubject = A.Fake<ProductSubject>();

            _unitOfWork = new UnitOfWork(_context, _factory);

            _notificationObserver = A.Fake<INotificationObserver<Product>>();
        }

        //Testa alla metoder genom Factory här



        [Fact]
        public async Task CompleteAsync_CallsMethod_MustHaveHappened()
        {
            //Arrange
            A.CallTo(() => _unitOfWork.CompleteAsync()).Returns(Task.CompletedTask);


            //Act
            await _unitOfWork.CompleteAsync();

            //Assert

            A.CallTo(() => _unitOfWork.CompleteAsync()).MustHaveHappened();

        }

        [Fact]
        public void Dispose__CallsMethod_MustHaveHappened()
        {
            //Arrange

            A.CallTo(() => _unitOfWork.Dispose());

            //Act
            _unitOfWork.Dispose();

            //Assert
            A.CallTo(() => _unitOfWork.Dispose()).MustHaveHappened();

        }

        [Fact]
        public async Task Repository_CallsMethod_MustHaveHappenedOnce<T>()
        {
            //Arrange
            
            //Act

            var result = _unitOfWork._repositoryFactory.CreateRepository<Product>();

            //Assert
            Assert.IsAssignableFrom<IRepository<Product>>(result);

        }


        [Fact]
        public void NotifyProductAdded_CallsObserverUpdate()
        {
            //Arrange
           var product = new Product { Id = 1, Name = "Test" };

            //Skapar en mock av INotificationObserver
           

            //Skapar en instans av ProductSubject och lägger till mock - observatören
            var productSubject = new ProductSubject();
            productSubject.Attach(_notificationObserver);

            //Injicerar vårt eget ProductSubject i UnitOfWork
           

            //Act
            _unitOfWork.NotifyProductAdded(product);

           // Assert
            //Verifierar att Update-metoden kallades på vår mock-observatör
            A.CallTo(() => _notificationObserver.Update(product)).MustHaveHappened();
        }
    }
}
