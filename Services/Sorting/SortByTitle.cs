using GameLibrary.Models;

namespace GameLibrary.Services.Sorting
{
    public class SortByTitle : ISortStrategy
    {
        public IEnumerable<Game> Sort(IEnumerable<Game> games)
        {
            return games.OrderBy(g => g.Title);
        }
    }
}