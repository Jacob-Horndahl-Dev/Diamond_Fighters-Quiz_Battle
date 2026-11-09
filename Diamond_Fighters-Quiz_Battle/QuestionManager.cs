using System.Text;

namespace Diamond_Fighters_Quiz_Battle
{
    internal class QuestionManager
    {
        public List<Question> questions { get; private set; } = [];
        public void LoadQuestions() //laddar frågorna från json-fil och lägger till i listan. //blandar också frågorna
        {

        }
        public List<string> GetCategories()
        {
            List<string> categories = []; //Placeholder
            return categories;
        }        
        public int RemainingQuestionsInCategory()
        {
            return 0; //placeholder
        }
        public Question GetQuestion(string chosenCategory)
        {
            return questions[0]; //Placeholder
        }
    }
}
