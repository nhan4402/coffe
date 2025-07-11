﻿using System.ComponentModel.DataAnnotations;

namespace Shopping_Coffee.Models
{
    public class BrandModel
    {
        [Key] 
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }

    }
}
