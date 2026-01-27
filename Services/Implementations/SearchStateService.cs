namespace MaillotStore.Services.Implementations
{
    public class SearchStateService
    {
        public string? CurrentSearchTerm { get; private set; }

        public event Action? OnSearchTermChanged;

        public void SetSearchTerm(string term)
        {
            CurrentSearchTerm = term;
            OnSearchTermChanged?.Invoke();
        }
    }
}
