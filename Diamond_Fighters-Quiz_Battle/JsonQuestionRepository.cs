using System.Text.Json;

namespace Diamond_Fighters_Quiz_Battle
{
    internal class JsonQuestionRepository : IQuestionRepository
    {
        private string filePath = "Resources/questions.json"; //sökvägen till filen. la i mapp resources för det känns mer städat.
        public List<Question> LoadQuestions()
        {            
            if (!File.Exists(filePath))
            {
                Console.WriteLine("FATAL ERROR: FILE NOT FOUND. QUESTION DATABASE COULD NOT BE IMPORTED...");
                return new List<Question>();
            }

            try
            {
                string jsonQuestionsString = File.ReadAllText(filePath);
                var questions = JsonSerializer.Deserialize<List<Question>>(jsonQuestionsString);
                return questions ?? new List<Question>(); //Om det blir fel i listan gör ?? att en tom lista returneras.
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"FATAL ERROR: JSON-filen felaktig formaterad: {ex.Message}");
                return new List<Question>();
            }
        }
    }
}
