using E_Commerce.ApplicationLayer.IService;
using E_Commerce.DomainLayer.Interfaces;

namespace E_Commerce.ApplicationLayer.Service
{
    public class ProductService :IProductService
    {
        private readonly IUnitOfWork unitOfWork;

        public ProductService(IUnitOfWork unitOfWork )
        {
            this.unitOfWork = unitOfWork;
        }
    }
}
