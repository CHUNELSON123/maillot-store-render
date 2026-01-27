using MaillotStore.Models;
using System;

namespace MaillotStore.Services.Implementations
{
    public class OrderStateService
    {
        // --- START: Added Staging Logic ---
        private Order? _stagedOrder;

        public void StageOrder(Order order)
        {
            _stagedOrder = order;
        }

        public Order? GetStagedOrder()
        {
            var order = _stagedOrder;
            _stagedOrder = null; // Important: Clear the order after it's been retrieved
            return order;
        }
        // --- END: Added Staging Logic ---

        public event Action? OnOrderPlaced;

        public void NotifyOrderPlaced() => OnOrderPlaced?.Invoke();
    }
}