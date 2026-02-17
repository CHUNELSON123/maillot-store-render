using MaillotStore.Models;
using System;

namespace MaillotStore.Services.Interfaces
{
    public interface IOrderStateService
    {
        Order CurrentOrder { get; set; }
        bool HasOrder { get; }

        void SetOrder(Order order);
        void ResetOrder();

        // --- STAGING METHODS ---
        void StageOrder(Order order);
        Order? GetStagedOrder();

        // --- ADDED: Missing method to fix Checkout error ---
        void ClearStagedOrder();
        // --------------------------------------------------

        event Action? OnOrderPlaced;
        void NotifyOrderPlaced();
    }
}