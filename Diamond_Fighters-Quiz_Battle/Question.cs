
namespace Diamond_Fighters_Quiz_Battle
{
    internal class Question
    {
        public string Type { get; set; } //enum? möjliga värden för datafält
        public string Category { get; set; } 
        public string QuestionText {  get; set; }
        //public string AnswerOptions { get; set; } //Lägger i subklass
        //public string CorrectAnswer { get; set; } //Lägger i subklass
        public Question()
        {

        }
        public void CheckAnswer()
        {

        }
        public void GetScore()
        {
        
        }
    }
}
