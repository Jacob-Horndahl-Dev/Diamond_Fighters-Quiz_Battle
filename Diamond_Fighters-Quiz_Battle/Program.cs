using Diamond_Fighters_Quiz_Battle;

internal class Program
{
    private static void Main(string[] args)
    {
        var highScores = new HighScoreManager("data/highscores.json");
        var menu = new Menu(highScores);
        menu.ShowMenu();
    }
}                                                           
