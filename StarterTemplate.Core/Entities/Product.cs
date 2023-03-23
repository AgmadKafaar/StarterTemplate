namespace StarterTemplate.Core.Entities
{
    public class Product
    {
        public string Domain { get; set; }
        public string GroupName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string PID { get; set; }
        public string Regdate { get; set; }
        public ProductStatus Status { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}