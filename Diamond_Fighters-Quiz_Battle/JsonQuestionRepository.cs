using System.Text.Json;

namespace Diamond_Fighters_Quiz_Battle
{
    internal class JsonQuestionRepository : IQuestionRepository
    {
        private string filePath = "Resources/questions.json"; //sökvägen till filen
        public List<Question> LoadQuestions()
        {
            throw new NotImplementedException();
        }
    }
}
