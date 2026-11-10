using System.Text.Json;
using System.Text.Json.Serialization;

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

                var questionJsonFileDto = JsonSerializer.Deserialize<QuestionJsonFileDto>(jsonQuestionsString);
                var questions = new List<Question>();

                foreach (var jsonCategoryDto in questionJsonFileDto.Categories)
                {
                    foreach (var i in jsonCategoryDto.Questions)
                    {
                        var question = new Question
                        {
                            QuestionText = i.QuestionText,
                            Category = jsonCategoryDto.Category, // Hämtas från kategorins "name" i JSON
                            DifficultyLevel = i.DifficultyLevel,
                            CorrectAnswer = i.CorrectAnswer,
                            AnswerOptions = i.AnswerOptions //fel måste fixa så att den sorterar olika sorters frågor på typ
                        };

                        questions.Add(question);
                    }

                    return questions ?? new List<Question>(); //Om det blir fel i listan gör ?? att en tom lista returneras.
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"FATAL ERROR: JSON-filen felaktig formaterad: {ex.Message}");
                return new List<Question>();
            }
        }
    }
    internal class QuestionJsonFileDto //dto betyder data transfer object
    {
        [JsonPropertyName("categories")]        //På grund av att json filen var uppbyggd med kategorier måste vi avkoda dem först.
        public List<JsonCategoryDto> Categories { get; set; }
    }
    internal class JsonCategoryDto //Hämtar namnet på frågorna och kategorin
    {
        [JsonPropertyName("name")]
        public string Category { get; set; }

        [JsonPropertyName("questions")]
        public List<QuestionJsonDto> Questions { get; set; }
    }
    internal class QuestionJsonDto //Hämtar datafälten i frågorna
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("difficulty")]
        public string DifficultyLevel { get; set; }

        [JsonPropertyName("text")]
        public string QuestionText { get; set; }

        [JsonPropertyName("correctIndex")]
        public string CorrectAnswer { get; set; }

        [JsonPropertyName("options")]
        public List<string> AnswerOptions { get; set; }
    }
}

// Min question:
/*public string Type { get; init; } //Flervalsfråga, true/false fråga. Använda enum? möjliga värden för datafält
public string Category { get; init; } //Programmering, Geografi, Allmänbildning
public string DifficultyLevel { get; init; } //Lätt, medel, svår
public string QuestionText { get; init; } //Texten med själva frågan
public virtual string CorrectAnswer { get; init; } //Subklassen kan hantera svaren på sitt sätt*/
/*public List<string> AnswerOptions { get; init; } = []; //Lista med svarsalternativen */

//Jsonfilen:
/*"categories": [
    {
      "name": "Programmering",
      "questions": [
        {
          "type": "multipleChoice",
          "text": "Vilket tecken används i C# för att en klass ska ärva från en annan klass?",
          "options": [
            "->",
            "=>",
            ":",
            "::"
          ],
          "correctIndex": 2,
          "difficulty": "easy"*/