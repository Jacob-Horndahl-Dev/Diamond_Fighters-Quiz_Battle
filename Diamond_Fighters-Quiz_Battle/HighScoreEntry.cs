namespace Diamond_Fighters_Quiz_Battle
{
    // En rad på topplistan: namn, poäng, antal rätt och datum.
    internal class HighScoreEntry
    {
        public string Name { get; set; } = "";
        public int Score { get; set; }
        public int CorrectAnswers { get; set; }
        public string Date { get; set; } = "";
    }
}