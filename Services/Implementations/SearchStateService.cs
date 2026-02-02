using MaillotStore.Services.Interfaces;
using System;

namespace MaillotStore.Services.Implementations
{
    public class SearchStateService : ISearchStateService
    {
        // Property is read-only public, set privately via method
        public string? CurrentSearchTerm { get; private set; }

        public event Action? OnSearchTermChanged;

        public void SetSearchTerm(string term)
        {
            CurrentSearchTerm = term;
            // Notify subscribers (like the Shop page) that search changed
            OnSearchTermChanged?.Invoke();
        }
    }
}