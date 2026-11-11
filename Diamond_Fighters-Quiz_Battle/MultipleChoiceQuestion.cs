
namespace Diamond_Fighters_Quiz_Battle
{
    internal class MultipleChoiceQuestion : Question
    {
        public List<string> AnswerOptions { get; init; } = []; //Lista med svarsalternativen 
        public override void ShowQuestionAnswerOptions()
        {
            if (AnswerOptions != null)
            {
                for (int i = 0; i < AnswerOptions.Count; i++)
                {
                    // Skriver ut alternativen med 1-4 för spelaren
                    Console.WriteLine($"{i + 1}. {AnswerOptions[i]}");
                }
            }
        }
        public override bool CheckAnswer(string guessedAnswer) //Kontroll av svar när flera alternativ finns.
        {            
            if (int.TryParse(guessedAnswer, out int parsedGuess)) //Kontroll av svar via index (1-4)
            {
                int parsedGuessIndex = parsedGuess - 1; //ändrar användarens svar till index
                if (int.TryParse(CorrectAnswer, out int parsedCorrectAnswer))
                {
                    return parsedGuessIndex == parsedCorrectAnswer; //om svaret är lika med rätt svar (båda omvandlade till int från string)
                }
            }
            if (int.TryParse(CorrectAnswer, out int parsedCorrectAnswerFullTextIndex)) //kontroll ifall användarens skriver in hela ordet som är rätt
            {
                string correctAnswer = AnswerOptions[parsedCorrectAnswerFullTextIndex];
                return guessedAnswer.Trim().ToLower() == correctAnswer.Trim().ToLower(); //om svaret utskrivet är rätt
            }
                return false; //om svaret inte är ett giltigt index 1-4, skrivet helt rätt eller är fel returneras false.
        }
    }
}
