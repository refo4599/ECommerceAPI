using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Domain.Entities;

public class Cart : BaseEntity
{
    public int UserId { get; set; }
    public int BranchId { get; set; }

    public User User { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public ICollection<CartItem> CartItems { get; set; } = [];
}
