using E_Commerce.DomainLayer.Entities;
using E_Commerce.InfrastructureLayer.Data.DBContext;
using E_Commerce.InfrastructureLayer.Data.DBContext.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.InfrastructureLayer.Data.GenericClass
{
    public class GenericProductRepo : Repository<Product>
    {
        public GenericProductRepo(ApplicationDBContext context) 
            : base(context)
        {
        }
    }
}
