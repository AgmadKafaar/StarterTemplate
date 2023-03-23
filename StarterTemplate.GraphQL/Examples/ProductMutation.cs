using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using StarterTemplate.Core.Entities;

namespace StarterTemplate.GraphQL.Examples
{
    /// <summary>
    /// Service mutation endpoints
    /// </summary>
    [ExtendObjectType("Mutation")]
    // if you have multiple mutation types, then extending the mutation object is the way to go.
    public class ProductMutation
    {
        private readonly List<Product> _products;

        public ProductMutation()
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
        /// Updates an in memory product.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="eventSender"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ProductEditPayload> UpdateProduct(ProductEditInput input, [Service] ITopicEventSender eventSender,
            CancellationToken cancellationToken)
        {
            var productToUpdate = _products.FirstOrDefault(x => x.Id == input.Id);
            ProductEditPayload result;
            if (productToUpdate == null) result = new ProductEditPayload() { Product = null, Success = false };
            else
            {
                productToUpdate.Name = input.Name;
                result = new ProductEditPayload() { Product = productToUpdate, Success = true };
            }

            await eventSender.SendAsync(input.Topic, result, cancellationToken);
            return result;
        }
    }
}