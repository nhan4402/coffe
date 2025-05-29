using System.ComponentModel.DataAnnotations;

namespace Shopping_Coffee.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required,MinLength(4,ErrorMessage ="Yêu cầu nhập tên")]
        public string Name { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }

    }
}
