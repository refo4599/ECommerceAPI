using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Domain.Enums;
public enum UserRole
{
    Customer,
    Admin
}

public enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    Completed,
    Cancelled
}