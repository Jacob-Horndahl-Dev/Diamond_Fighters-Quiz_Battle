
using System.Collections.Generic;
using System.Linq;

namespace Diamond_Fighters_Quiz_Battle
{
    public class ScoreManager
    {
        public int CalculateScore(int difficulty, int streak)
        {
            int basePoints = difficulty switch
            {
                1 => 10,
                2 => 20,
                3 => 30,
                _ => 0
            };

            if (basePoints == 0)
            {
                return 0;
            }

            int bonus = System.Math.Min(streak * 5, 20);

            return basePoints + bonus;
        }

        
public List<Player> DetermineWinner(List<Player> players)
{
    if (players.Count == 0)
    {
        return new List<Player>();
    }

    int highestScore = players.Max(p => p.Score);

    List<Player> topPlayers = players
        .Where(p => p.Score == highestScore)
        .ToList();

    int mostCorrectAnswers = topPlayers
        .Max(p => p.CorrectAnswers);

    return topPlayers
        .Where(p => p.CorrectAnswers == mostCorrectAnswers)
        .ToList();
}

    }
}
