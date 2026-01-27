using MaillotStore.Models;
using MaillotStore.Services.Interfaces; // <--- Import the interface here
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaillotStore.Services.Implementations
{
    // DELETE "public interface ICartService { ... }" from here. It is now in its own file.

    public class CartService : ICartService
    {
        private readonly List<OrderItem> _cartItems = new();

        public event Action OnChange;

        public List<OrderItem> GetCartItems() => _cartItems;

        public void AddToCart(Product product, int quantity, string size, string? customName, int? customNumber)
        {
            var existingItem = _cartItems.FirstOrDefault(i =>
                i.Product.ProductId == product.ProductId &&
                i.Size == size &&
                i.CustomName == customName &&
                i.CustomNumber == customNumber);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _cartItems.Add(new OrderItem
                {
                    Product = product,
                    Quantity = quantity,
                    Price = product.Price,
                    Size = size,
                    CustomName = customName,
                    CustomNumber = customNumber
                });
            }
            NotifyStateChanged();
        }

        public void RemoveFromCart(OrderItem item)
        {
            _cartItems.Remove(item);
            NotifyStateChanged();
        }

        public void UpdateQuantity(OrderItem item, int quantity)
        {
            var existingItem = _cartItems.FirstOrDefault(i =>
                i.Product.ProductId == item.Product.ProductId &&
                i.Size == item.Size &&
                i.CustomName == item.CustomName &&
                i.CustomNumber == item.CustomNumber);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity;
                if (existingItem.Quantity <= 0)
                {
                    _cartItems.Remove(existingItem);
                }
            }
            NotifyStateChanged();
        }

        public void UpdateSize(OrderItem item, string size)
        {
            var existingItem = _cartItems.FirstOrDefault(i => i.Product.ProductId == item.Product.ProductId);
            if (existingItem != null)
            {
                existingItem.Size = size;
                NotifyStateChanged();
            }
        }

        public void ClearCart()
        {
            _cartItems.Clear();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}