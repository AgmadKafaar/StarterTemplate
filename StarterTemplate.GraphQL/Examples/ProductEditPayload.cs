using StarterTemplate.Core.Entities;

namespace StarterTemplate.GraphQL.Examples
{
    /// <summary>
    /// Result of editing a product
    /// </summary>
    public class ProductEditPayload
    {
        /// <summary>
        /// Indicates whether update was successful or not
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Product returned in payload after update
        /// </summary>
        public Product Product { set; get; }
    }
}