public class Player
{
    public string Name { get; set; } = "";
    public int Score { get; set; }
    public int Streak { get; set; }

    public void AddPoints(int points)
    {
        Score += points;
    }

    public void ResetStreak()
    {
        Streak = 0;
    }
}