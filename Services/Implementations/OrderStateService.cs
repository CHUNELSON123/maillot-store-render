using MaillotStore.Models;
using MaillotStore.Services.Interfaces;
using System;

namespace MaillotStore.Services.Implementations
{
    public class OrderStateService : IOrderStateService
    {
        public Order CurrentOrder { get; set; }

        public bool HasOrder => CurrentOrder != null;

        public void SetOrder(Order order)
        {
            CurrentOrder = order;
        }

        public void ResetOrder()
        {
            CurrentOrder = null;
        }

        // --- Staging Logic ---
        private Order? _stagedOrder;

        public void StageOrder(Order order)
        {
            _stagedOrder = order;
        }

        public Order? GetStagedOrder()
        {
            // Note: You currently "pop" (clear) the order when getting it here.
            // If you want it to persist until explicitly cleared, remove the '_stagedOrder = null;' line.
            var order = _stagedOrder;
            // _stagedOrder = null; // Commented out to allow explicit clearing via ClearStagedOrder if preferred, 
            // or keep it if you want "read-once" behavior. 
            // For safety with the new method, usually we just return it here.
            return order;
        }

        // --- FIX: Added the missing method required by the Interface ---
        public void ClearStagedOrder()
        {
            _stagedOrder = null;
        }
        // --------------------------------------------------------------

        // --- GLOBAL NOTIFICATION LOGIC ---

        // 1. We create a STATIC event. "Static" means it is shared by ALL users on the server.
        private static Action? _globalOrderPlaced;

        // 2. We implement the Interface event using "add/remove".
        //    When the Dashboard subscribes to this service, we actually subscribe them 
        //    to the global static event.
        public event Action? OnOrderPlaced
        {
            add => _globalOrderPlaced += value;
            remove => _globalOrderPlaced -= value;
        }

        // 3. When triggered, it invokes the global event, notifying everyone.
        public void NotifyOrderPlaced() => _globalOrderPlaced?.Invoke();
    }
}