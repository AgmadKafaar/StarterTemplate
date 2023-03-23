namespace StarterTemplate.GraphQL.Examples
{
    /// <summary>
    /// input payload to edit a product
    /// </summary>
    public class ProductEditInput
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public string Id { set; get; }
        /// <summary>
        /// Name to update
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Used for Subscriptions to send messages to in pub sub
        /// </summary>
        public string Topic { get; set; }
    }
}
