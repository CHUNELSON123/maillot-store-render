using System;

namespace MaillotStore.Services.Interfaces
{
    public interface ISearchStateService
    {
        // Allow reading the term
        string? CurrentSearchTerm { get; }

        // Event for updates
        event Action? OnSearchTermChanged;

        // Method to set the term (This was missing!)
        void SetSearchTerm(string term);
    }
}