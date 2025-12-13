namespace MaillotStore.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Foreign Key link to Product
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}