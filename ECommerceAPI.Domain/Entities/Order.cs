using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Domain.Enums;

namespace ECommerceAPI.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public int BranchId { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}