using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceAPI.Infrastructure.Hubs;

public class StockHub : Hub
{
    public async Task JoinBranch(int branchId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }

    public async Task LeaveBranch(int branchId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"branch_{branchId}");
    }
}