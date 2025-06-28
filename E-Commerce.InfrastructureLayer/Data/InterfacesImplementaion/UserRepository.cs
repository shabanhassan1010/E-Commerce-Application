
using E_Commerce.DomainLayer.Entities;
using E_Commerce.DomainLayer.Interfaces;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace E_Commerce.InfrastructureLayer.Data.InterfacesImplementaion
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly ApplicationDBContext context;
        public UserRepository(ApplicationDBContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<User?> GetUser(int id)
        {
            return await context.Users.FindAsync(id);
        }
    }
}
