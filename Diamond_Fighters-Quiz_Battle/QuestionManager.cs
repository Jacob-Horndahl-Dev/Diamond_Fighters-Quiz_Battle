using System.Text;

namespace Diamond_Fighters_Quiz_Battle
{
    internal class QuestionManager
    {
        public List<Question> questions { get; private set; } = [];
        private List<string> categoriesList = []; //Håller reda på att kategorierna finns även om frågorna i kategorin tar slut.
        private IQuestionRepository _questionRepository; //private så ingen annan del av programmet kan ändra på frågorna
        public bool OutOfQuestions => questions.Count == 0;

        public QuestionManager()
        {            
            _questionRepository = new JsonQuestionRepository(); //Här anges vilken källa till frågorna, kan byta ut Json till annan klass, tex något på webben eller annan filtyp.
            LoadQuestions();
        }
        public void LoadQuestions() //laddar frågorna från json-fil och lägger till i listan. //blandar också frågorna
        {
            questions = _questionRepository.LoadQuestions(); //Fyller den tomma listan med frågor från den valda källan.
            questions = questions.OrderBy(q => Guid.NewGuid()).ToList(); //Linq funktionen Guid.NewGuid() blandar listan, och vi slipper göra en loop för det!
            categoriesList = questions.Select(q => q.Category).Distinct().ToList(); //Laddar en lista med alla kategorier som fanns med i början av spelet.
        }
        public List<string> GetCategories()
        {
            return categoriesList; //returnerar en lista med kategorierna som finns. tex: List<string> categories = questionManager.GetCategories();
        }
        public int RemainingQuestionsInCategory(string category) //Skicka in en kategori och den returnerar hur många frågor som är kvar i den kategorin.
        {
            if (category == null) { return 0; } //om frågor i en kategori tagit slut skris noll ut och programmet kraschar inte.
            return questions.Count(q => q.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }
        public Question GetQuestion(string chosenCategory) //Skicka in en kategori som spelaren valt och en fråga kommer tillbaka.
        {
            if (OutOfQuestions) //ifall frågorna är slut kraschar inte programmet. Men användaren bör stoppas från att dra frågor från en kategori om den är slut!
            {
                LoadQuestions();
            }
            var question = questions.FirstOrDefault(q => q.Category.Equals(chosenCategory, StringComparison.OrdinalIgnoreCase));
            if (question == null)
            {
                Console.WriteLine("DEBUG: Bugg inträffad. Spelaren ska förhindras att välja en fråga från en kategori som är slut.");
                Console.WriteLine("DEBUG: Om alla frågor är slut får spelarna automatiskt nya frågor att välja mellan.");
                return null; // Avbryt och returnera null så programmet inte kraschar
            }
            questions.Remove(question); //tar bort frågan ur listan så den inte ställs igen.
            return question;            
        }
    }
}
