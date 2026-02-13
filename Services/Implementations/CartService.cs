using MaillotStore.Data;
using MaillotStore.Models;
using MaillotStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaillotStore.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;

        public string GuestId { get; private set; } = string.Empty;

        public event Action OnChange;

        public CartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SetGuestId(string guestId)
        {
            GuestId = guestId;
            NotifyStateChanged();
        }

        // --- READ: Get items from Database ---
        public List<OrderItem> GetCartItems()
        {
            if (string.IsNullOrEmpty(GuestId)) return new List<OrderItem>();

            // 1. Fetch from DB with ALL related data needed for discounts
            var dbItems = _context.CartItems
                .Include(c => c.Product)
                    .ThenInclude(p => p.Team)    // Needed for Team Discounts
                .Include(c => c.Product)
                    .ThenInclude(p => p.League)  // Needed for League Discounts
                .Where(c => c.GuestId == GuestId)
                .ToList();

            // 2. Convert to OrderItem
            return dbItems.Select(c => new OrderItem
            {
                ProductId = c.ProductId,
                Product = c.Product,
                Quantity = c.Quantity,
                Size = c.Size,
                CustomName = c.CustomName,
                CustomNumber = c.CustomNumber,

                // --- FIX: Use EffectivePrice instead of original Price ---
                // This ensures Promo/Team/League discounts are applied in the cart
                Price = c.Product.EffectivePrice
                // --------------------------------------------------------
            }).ToList();
        }

        // --- CREATE: Add to Database ---
        public void AddToCart(Product product, int quantity, string size, string? customName, int? customNumber)
        {
            if (string.IsNullOrEmpty(GuestId)) return;

            var existingItem = _context.CartItems.FirstOrDefault(i =>
                i.GuestId == GuestId &&
                i.ProductId == product.ProductId &&
                i.Size == size &&
                i.CustomName == customName &&
                i.CustomNumber == customNumber);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newItem = new CartItem
                {
                    GuestId = GuestId,
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    Size = size,
                    CustomName = customName,
                    CustomNumber = customNumber,
                    DateCreated = DateTime.UtcNow
                };
                _context.CartItems.Add(newItem);
            }

            _context.SaveChanges();
            NotifyStateChanged();
        }

        // --- DELETE: Remove from Database ---
        public void RemoveFromCart(OrderItem item)
        {
            if (string.IsNullOrEmpty(GuestId)) return;

            var dbItem = _context.CartItems.FirstOrDefault(i =>
                i.GuestId == GuestId &&
                i.ProductId == item.Product.ProductId &&
                i.Size == item.Size &&
                i.CustomName == item.CustomName &&
                i.CustomNumber == item.CustomNumber);

            if (dbItem != null)
            {
                _context.CartItems.Remove(dbItem);
                _context.SaveChanges();
                NotifyStateChanged();
            }
        }

        // --- UPDATE: Change Quantity in Database ---
        public void UpdateQuantity(OrderItem item, int quantity)
        {
            if (string.IsNullOrEmpty(GuestId)) return;

            var dbItem = _context.CartItems.FirstOrDefault(i =>
                i.GuestId == GuestId &&
                i.ProductId == item.Product.ProductId &&
                i.Size == item.Size &&
                i.CustomName == item.CustomName &&
                i.CustomNumber == item.CustomNumber);

            if (dbItem != null)
            {
                dbItem.Quantity = quantity;
                if (dbItem.Quantity <= 0)
                {
                    _context.CartItems.Remove(dbItem);
                }
                _context.SaveChanges();
                NotifyStateChanged();
            }
        }

        // --- UPDATE: Change Size in Database ---
        public void UpdateSize(OrderItem item, string size)
        {
            if (string.IsNullOrEmpty(GuestId)) return;

            var dbItem = _context.CartItems.FirstOrDefault(i =>
                i.GuestId == GuestId &&
                i.ProductId == item.Product.ProductId &&
                i.Size == item.Size &&
                i.CustomName == item.CustomName &&
                i.CustomNumber == item.CustomNumber);

            if (dbItem != null)
            {
                dbItem.Size = size;
                _context.SaveChanges();
                NotifyStateChanged();
            }
        }

        public void ClearCart()
        {
            if (string.IsNullOrEmpty(GuestId)) return;

            var userItems = _context.CartItems.Where(c => c.GuestId == GuestId);
            _context.CartItems.RemoveRange(userItems);
            _context.SaveChanges();
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}