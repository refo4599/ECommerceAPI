using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Domain.Entities;

public class BranchProduct : BaseEntity
{
    public int BranchId { get; set; }
    public int ProductId { get; set; }
    public int Stock { get; set; } = 0;
    public decimal? PriceOverride { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Branch Branch { get; set; } = null!;
    public Product Product { get; set; } = null!;
}