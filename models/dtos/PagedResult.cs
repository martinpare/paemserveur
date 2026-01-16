using System.Collections.Generic;

namespace serveur.Models.Dtos
{
    /// <summary>
    /// Classe générique pour les résultats paginés
    /// </summary>
    /// <typeparam name="T">Type des éléments</typeparam>
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
