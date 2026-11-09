
namespace Diamond_Fighters_Quiz_Battle
{
    internal class Question //init är en skrivskyddad setter, typ som private, men json kan inte läsas in direkt till private fält.
    {
        public string Type { get; init; } //Flervalsfråga, true/false fråga. Använda enum? möjliga värden för datafält
        public string Category { get; init; } //Programmering, Geografi, Allmänbildning
        public string DifficultyLevel { get; init; } //Lätt, medel, svår
        public string QuestionText { get; init; } //Håller denna som en string, så kan den hantera både flera svarsalternativ, true och false och fritextsvar (om det skulle behövas i framtiden).        
        public virtual string CorrectAnswer { get; init; } //Subklassen hanterar svaren på sitt sätt
        public Question()
        {
            //Måste jag ha en konstruktor? Frågorna laddas in via json
        }
        public virtual bool CheckAnswer(string guessedAnswer) //virtual när det finns en "standard" metod, abstract om den ska vara helt tom och subklasserna bestämmer helt själva hur det fungerar.
        {
            return false; //placeholder
        }
        public void GetScore()
        {
        
        }
    }
}
