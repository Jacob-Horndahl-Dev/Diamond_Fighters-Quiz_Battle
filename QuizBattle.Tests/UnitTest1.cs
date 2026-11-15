using Xunit;
using Diamond_Fighters_Quiz_Battle;

namespace QuizBattle.Tests;

public class UnitTest1
{
    [Fact]
    public void EasyQuestionGives10Points()
    {
        ScoreManager scoreManager = new ScoreManager();

        int points = scoreManager.CalculateScore(1, 0);

        Assert.Equal(10, points);
    }
[Theory]
[InlineData(2, 0, 20)]
[InlineData(3, 0, 30)]
[InlineData(1, 1, 15)]
[InlineData(1, 2, 20)]
[InlineData(1, 4, 30)]
[InlineData(1, 5, 30)]
public void CalculateScoreGivesCorrectPoints(
    int difficulty, int streak, int expected)
{
    ScoreManager scoreManager = new ScoreManager();

    int actual = scoreManager.CalculateScore(difficulty, streak);

    Assert.Equal(expected, actual);
}
[Fact]
public void PlayerWithHighestScoreWins()
{
    var players = new List<Player>
    {
        new Player { Name = "Hannah", Score = 50 },
        new Player { Name = "Jacob", Score = 30 }
    };

    ScoreManager scoreManager = new ScoreManager();

    var winners = scoreManager.DetermineWinner(players);

    Assert.Single(winners);
    Assert.Equal("Hannah", winners[0].Name);
}

[Fact]
public void PlayersCanTie()
{
    var players = new List<Player>
    {
        new Player { Name = "Hannah", Score = 50 },
        new Player { Name = "Jacob", Score = 50 }
    };

    ScoreManager scoreManager = new ScoreManager();

    var winners = scoreManager.DetermineWinner(players);

    Assert.Equal(2, winners.Count);
}
[Fact]
public void AddPointsIncreasesScore()
{
    Player player = new Player { Name = "Hannah" };

    player.AddPoints(20);

    Assert.Equal(20, player.Score);
}

[Fact]
public void ResetStreakSetsStreakToZero()
{
    Player player = new Player { Name = "Hannah", Streak = 3 };

    player.ResetStreak();

    Assert.Equal(0, player.Streak);
}
[Fact]
public void GameRequiresAtLeastTwoPlayers()
{
    var players = new List<Player>
    {
        new Player { Name = "Hannah" }
    };

    var questions = new List<Question>();

    Assert.Throws<ArgumentException>(() =>
        new Game(players, questions, 3));
}
}
