using MaillotStore.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization; // <-- IMPORTANT: Required for ReferenceHandler
using System.Threading.Tasks;

namespace MaillotStore.Services.Implementations
{
    public class StateContainer
    {
        private readonly IJSRuntime _jsRuntime;

        // --- DEFINE OPTIONS TO HANDLE CYCLES ---
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false
        };

        public StateContainer(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public List<OrderItem> CartItems { get; private set; } = new();

        public event Action OnChange;

        public async Task AddToCart(Product product, int quantity, string size, string customName, int? customNumber)
        {
            var existingItem = CartItems.FirstOrDefault(i =>
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
                CartItems.Add(new OrderItem
                {
                    Product = product,
                    Quantity = quantity,
                    Price = product.Price,
                    Size = size,
                    CustomName = customName,
                    CustomNumber = customNumber
                });
            }

            await SaveState();
            NotifyStateChanged();
        }

        public async Task RemoveFromCart(OrderItem item)
        {
            var itemToRemove = CartItems.FirstOrDefault(i =>
                i.Product.ProductId == item.Product.ProductId &&
                i.Size == item.Size &&
                i.CustomName == item.CustomName &&
                i.CustomNumber == item.CustomNumber);

            if (itemToRemove != null)
            {
                CartItems.Remove(itemToRemove);
                await SaveState();
                NotifyStateChanged();
            }
        }

        public async Task UpdateCartItemQuantity(OrderItem item, int quantity)
        {
            var itemInCart = CartItems.FirstOrDefault(i =>
                i.Product.ProductId == item.Product.ProductId &&
                i.Size == item.Size &&
                i.CustomName == item.CustomName &&
                i.CustomNumber == item.CustomNumber);

            if (itemInCart != null)
            {
                itemInCart.Quantity = quantity;
                if (itemInCart.Quantity <= 0)
                {
                    CartItems.Remove(itemInCart);
                }
                await SaveState();
                NotifyStateChanged();
            }
        }

        public async Task UpdateCartItemSize(OrderItem item, string newSize)
        {
            var itemInCart = CartItems.FirstOrDefault(i =>
                i.Product.ProductId == item.Product.ProductId &&
                i.Size == item.Size &&
                i.CustomName == item.CustomName &&
                i.CustomNumber == item.CustomNumber);

            if (itemInCart != null)
            {
                var existingItemWithNewSize = CartItems.FirstOrDefault(i =>
                   i.Product.ProductId == item.Product.ProductId &&
                   i.Size == newSize &&
                   i.CustomName == item.CustomName &&
                   i.CustomNumber == item.CustomNumber &&
                   i != itemInCart);

                if (existingItemWithNewSize != null)
                {
                    existingItemWithNewSize.Quantity += itemInCart.Quantity;
                    CartItems.Remove(itemInCart);
                }
                else
                {
                    itemInCart.Size = newSize;
                }

                await SaveState();
                NotifyStateChanged();
            }
        }

        public async Task ClearCart()
        {
            CartItems.Clear();
            await SaveState();
            NotifyStateChanged();
        }

        public async Task SaveState()
        {
            // --- UPDATED: Pass _jsonOptions here ---
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "maillot_store_cart", JsonSerializer.Serialize(CartItems, _jsonOptions));
        }

        public async Task LoadState()
        {
            var cartJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "maillot_store_cart");
            if (!string.IsNullOrEmpty(cartJson))
            {
                try
                {
                    // --- UPDATED: Pass _jsonOptions here ---
                    CartItems = JsonSerializer.Deserialize<List<OrderItem>>(cartJson, _jsonOptions) ?? new List<OrderItem>();
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error deserializing cart: {ex.Message}");
                    CartItems = new List<OrderItem>();
                    await SaveState();
                }
            }
            else
            {
                CartItems = new List<OrderItem>();
            }
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}