using GameLibrary.Models;
using GameLibrary.Services.Sorting;

namespace GameLibrary.Services
{
    public class StatisticsService
    {
        public ISortStrategy SortStrategy { get; set; }

        public StatisticsService(ISortStrategy sortStrategy)
        {
            SortStrategy = sortStrategy;
        }

        public IEnumerable<Game> GetSortedGames(IEnumerable<Game> games)
        {
            return SortStrategy.Sort(games);
        }
    }
}