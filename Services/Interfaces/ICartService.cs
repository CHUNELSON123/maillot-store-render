using MaillotStore.Models;
using System;
using System.Collections.Generic;

namespace MaillotStore.Services.Interfaces
{
    public interface ICartService
    {
        event Action OnChange;
        List<OrderItem> GetCartItems();
        void AddToCart(Product product, int quantity, string size, string? customName, int? customNumber);
        void RemoveFromCart(OrderItem item);
        void UpdateQuantity(OrderItem item, int quantity);
        void UpdateSize(OrderItem item, string size);
        void ClearCart();

        // --- ADDED: Method to set the Guest Cookie ID ---
        void SetGuestId(string guestId);
    }
}