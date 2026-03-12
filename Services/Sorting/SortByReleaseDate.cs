using System.Linq;
using GameLibrary.Models;

namespace GameLibrary.Services.Sorting
{
    public class SortByReleaseDate : ISortStrategy
    {
        public IEnumerable<Game> Sort(IEnumerable<Game> games)
        {
            return games.OrderByDescending(g => g.ReleaseDate);
        }
    }
}