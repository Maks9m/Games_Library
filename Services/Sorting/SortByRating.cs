using System.Linq;
using GameLibrary.Models;

namespace GameLibrary.Services.Sorting
{
    public class SortByRating : ISortStrategy
    {
        public IEnumerable<Game> Sort(IEnumerable<Game> games)
        {
            return games.OrderByDescending(g => g.Rating);
        }
    }
}