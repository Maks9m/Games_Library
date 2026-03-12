using GameLibrary.Models;

namespace GameLibrary.Services.Sorting
{
  public interface ISortStrategy
    {
        IEnumerable<Game> Sort(IEnumerable<Game> games);
    }
}