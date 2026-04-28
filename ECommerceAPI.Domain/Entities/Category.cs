using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<Product> Products { get; set; } = [];
}