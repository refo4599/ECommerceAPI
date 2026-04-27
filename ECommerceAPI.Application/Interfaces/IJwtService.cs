using System;
using System.Collections.Generic;
using System.Text;

using ECommerceAPI.Domain.Entities;

namespace ECommerceAPI.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}