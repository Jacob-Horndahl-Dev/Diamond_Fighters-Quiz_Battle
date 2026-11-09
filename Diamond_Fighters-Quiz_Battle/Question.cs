
namespace Diamond_Fighters_Quiz_Battle
{
    internal class Question
    {
        public string Type { get; private set; } //Flervalsfråga, true/false fråga. Använda enum? möjliga värden för datafält
        public string Category { get; private set; } //Programmering, Geografi, Allmänbildning
        public string DifficultyLevel { get; private set; } //Lätt, medel, svår
        public string QuestionText {  get; private set; } //Håller denna som en string, så kan den hantera både flera svarsalternativ, true och false och fritextsvar (om det skulle behövas i framtiden).
        public virtual string AnswerOptions { get; private set; } //Subklassen hanterar svaren på sitt sätt
        public virtual string CorrectAnswer { get; set; } //Subklassen hanterar svaren på sitt sätt
        public Question()
        {
            //Måste jag ha en konstruktor? Frågorna laddas in via json
        }
        public virtual void CheckAnswer(string guessedAnswer) //virtual när det finns en "standard" metod, abstract om den ska vara helt tom och subklasserna bestämmer helt själva hur det fungerar.
        {

        }
        public void GetScore()
        {
        
        }
    }
}
