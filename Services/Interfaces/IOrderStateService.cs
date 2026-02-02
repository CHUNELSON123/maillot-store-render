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

        // --- ADDED THESE METHODS ---
        void StageOrder(Order order);
        Order? GetStagedOrder();

        event Action? OnOrderPlaced;
        void NotifyOrderPlaced();
    }
}