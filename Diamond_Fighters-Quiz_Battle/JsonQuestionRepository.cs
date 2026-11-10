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
    internal class QuestionJsonDto //dto betyder data transfer object
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

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