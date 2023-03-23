using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotChocolate.Types;
using StarterTemplate.Core.Entities;

namespace StarterTemplate.GraphQL.Examples
{
    /// <summary>
    /// Product Query using In Memory List.
    /// Replace List with a repository or an API client.
    /// </summary>
    [ExtendObjectType("Query")]
    // if you have multiple query types, then extending the query object is the way to go.
    public class ProductQuery
    {
        private readonly List<Product> _products;

        public ProductQuery()
        {
            _products = new List<Product>()
            {
                new Product()
                {
                    Domain = "a",
                    GroupName = "b",
                    Id = "1",
                    Name = "Product 1",
                    PID = "1",
                    Regdate = "2021-01-01",
                    Status = ProductStatus.Active
                },
                new Product()
                {
                    Domain = "c",
                    GroupName = "d",
                    Id = "2",
                    Name = "Product 2",
                    PID = "1",
                    Regdate = "2021-01-01",
                    Status = ProductStatus.Active
                },
                new Product()
                {
                Domain = "e",
                GroupName = "f",
                Id = "3",
                Name = "Product 3",
                PID = "1",
                Regdate = "2021-01-01",
                Status = ProductStatus.Active
            }
            };
        }

        /// <summary>
        /// Get products by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<Product> GetProductById(string id)
        {
            var result = _products.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Get products by Service Type
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public Task<IEnumerable<Product>> GetProductByServiceType(ServiceType serviceType)
        {
            var result = _products.Where(x => x.ServiceType == serviceType);
            return Task.FromResult(result);
        }
        /// <summary>
        /// Get products by Status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task<IEnumerable<Product>> GetProductByStatus(ProductStatus status)
        {
            var result = _products.Where(x => x.Status == status);
            return Task.FromResult(result);
        }
    }
}