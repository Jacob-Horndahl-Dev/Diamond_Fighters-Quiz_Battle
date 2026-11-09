
namespace Diamond_Fighters_Quiz_Battle
{
    internal class Question //init är en skrivskyddad setter, typ som private, men json kan inte läsas in direkt till private fält.
    {
        public string Type { get; init; } //Flervalsfråga, true/false fråga. Använda enum? möjliga värden för datafält
        public string Category { get; init; } //Programmering, Geografi, Allmänbildning
        public string DifficultyLevel { get; init; } //Lätt, medel, svår
        public string QuestionText { get; init; } //Texten med själva frågan
        public virtual string CorrectAnswer { get; init; } //Subklassen kan hantera svaren på sitt sätt
        /*public Question()
        {
            //Måste jag ha en konstruktor? Frågorna laddas in via json. Går att lägga kontroll av null/whitespace här, men lägger det vid inläsningen för att slippa göra konstruktorer.
        }*/
        public virtual bool CheckAnswer(string guessedAnswer) //virtual när det finns en "standard" metod, abstract om den ska vara helt tom och subklasserna bestämmer helt själva hur det fungerar.
        {
            /*if (guessedAnswer == CorrectAnswer) return true;
            else return false;*/
            return guessedAnswer.Trim().ToLower() == CorrectAnswer.Trim().ToLower(); //jämför svaren utan whitespace och med gemener för att minska risken för fel
        }
        public int GetScore() //räknar ut hur mycket poäng en fråga är värd.
        {
            switch (DifficultyLevel?.Trim().ToLower()) // "?" gör att programmet inte kraschar när Trim() körs om fältet skulle vara null. Programmet går vidare med "null" som värde.
            {
                case "easy":
                    return 10;

                case "medium":
                    return 20;

                case "hard":
                    return 30;

                default:        //ifall något är felinmatat
                    return 10;
            }
            
        }
    }
}
