namespace UrbanBites.Application.DTOs.Category
{
    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public Guid BrandId { get; set; }
    }
}