namespace AdvanceFiltringSytem.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Guid? CategoryId { get; set; }
        public Guid? BrandId { get; set; }

        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? MarginPercent { get; set; }

        public bool IsStockManaged { get; set; }
        public decimal? StockQuantity { get; set; }
        public decimal? MinimumStockAlert { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
