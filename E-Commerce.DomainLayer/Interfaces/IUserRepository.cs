
using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using System.Linq.Expressions;

namespace E_Commerce.DomainLayer.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetUser(int id);
    }
}
