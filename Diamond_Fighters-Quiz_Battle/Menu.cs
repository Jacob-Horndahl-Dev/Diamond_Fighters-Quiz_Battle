namespace Diamond_Fighters_Quiz_Battle
{
    internal class Menu
    {
        // HighScoreManager som menyn anropar för att visa och spara topplistan.
        private readonly HighScoreManager highScores;

        // Sparar highscore-manager som skickas in när menyn skapas.
        public Menu(HighScoreManager highScores)
        {
            this.highScores = highScores;
        }

        // Visar menyn tills spelaren väljer att avsluta.
        public void ShowMenu()
        {
            ShowIntro();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("💎 DIAMOND FIGHTERS 💎");
                Console.WriteLine("⚔  Quiz Battle ⚔");
                Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━");
                Console.WriteLine();
                Console.WriteLine("  1. Spela");
                Console.WriteLine("  2. Topplista");
                Console.WriteLine("  3. Avsluta");
                Console.WriteLine();
                int choice = GetPlayerInput("Val: ", 1, 3);
                switch (choice)
                {
                    case 1:
                        InitializeGame();
                        break;
                    case 2:
                        highScores.ShowHighScores();
                        Console.WriteLine();
                        Console.Write("Tryck Enter för att gå tillbaka.");
                        Console.ReadLine();
                        break;
                    case 3:
                        Console.WriteLine();
                        Console.WriteLine("Avslutar.");
                        return;
                }
            }
        }

        // Startskärmen. Spelet går vidare när spelaren trycker Enter.
        private void ShowIntro()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Clear();
            Console.WriteLine("💎 DIAMOND FIGHTERS 💎");
            Console.WriteLine("⚔ THE ULTIMATE BATTLE ⚔");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine();
            Console.Write("[ TRYCK ENTER FÖR ATT STARTA ]");
            Console.ReadLine();
        }

        // Frågar tills spelaren skriver ett heltal inom intervallet.
        public int GetPlayerInput(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";

                if (int.TryParse(input, out int value) && value >= min && value <= max)
                {
                    return value;
                }

                Console.WriteLine("Skriv en siffra mellan " + min + " och " + max + ".");
            }
        }

        // Läser antal spelare, namn och rundor, kör spelet och sparar topplistan.
        public void InitializeGame()
        {
            int playerCount = GetPlayerInput("Antal spelare (2-4): ", 2, 4);
            List<string> names = ReadPlayerNames(playerCount);
            int rounds = GetPlayerInput("Antal rundor (1-10): ", 1, 10);

            Console.WriteLine("Spelare: " + string.Join(", ", names));
            Console.WriteLine("Rundor: " + rounds);
            List<Player> players = new List<Player>();
            foreach (string name in names)
            {
                players.Add(new Player { Name = name });
            }
            QuestionManager questionManager = new QuestionManager();
            questionManager.PrepareQuestions(rounds, playerCount);
            Game game = new Game(players, questionManager, rounds, this);
            game.StartGame();

            SaveResults(players);

            Console.WriteLine();
            Console.WriteLine("Resultaten sparades i data/highscores.json");
            Console.Write("Tryck Enter för att gå tillbaka.");
            Console.ReadLine();
        }

        // Läser ett namn per spelare. Tomma namn och dubbletter frågas om igen.
        private List<string> ReadPlayerNames(int playerCount)
        {
            List<string> names = new List<string>();

            while (names.Count < playerCount)
            {
                int number = names.Count + 1;
                Console.Write("Namn på spelare " + number + ": ");
                string name = (Console.ReadLine() ?? "").Trim();

                if (name == "")
                {
                    Console.WriteLine("Namnet får inte vara tomt.");
                    continue;
                }

                bool nameTaken = false;

                for (int i = 0; i < names.Count; i++)
                {
                    if (names[i].ToLower() == name.ToLower())
                    {
                        nameTaken = true;
                    }
                }

                if (nameTaken)
                {
                    Console.WriteLine("Namnet är redan taget.");
                    continue;
                }

                names.Add(name);
            }

            return names;
        }

        // Skriver en rad per spelare till data/highscores.json.
        private void SaveResults(List<Player> players)
        {
            foreach (Player player in players)
            {
                HighScoreEntry entry = new HighScoreEntry
                {
                    Name = player.Name,
                    Score = player.Score,
                    CorrectAnswers = player.CorrectAnswers,
                    Date = DateTime.Now.ToString("yyyy-MM-dd")
                };

                highScores.SaveHighScore(entry);
            }
        }

        // Spelaren väljer kategori, eller sista siffran för slump.
        public string? ChooseCategory(QuestionManager questionManager)
        {
            List<string> categories = questionManager.GetCategories()
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Where(c =>
                    questionManager.RemainingQuestionsInCategory(c) > 0)
                .ToList();

            if (categories.Count == 0)
            {
                Console.WriteLine("Det finns inga kategorier tillgängliga.");
                return null;
            }

            Console.WriteLine("\nVälj en kategori:");

            for (int i = 0; i < categories.Count; i++)
            {
                string category = categories[i];

                int remaining =
                    questionManager.RemainingQuestionsInCategory(category);

                Console.WriteLine(
                    $"{i + 1}. {category} ({remaining} frågor kvar)");
            }

            int randomOption = categories.Count + 1;

            Console.WriteLine($"{randomOption}. Slumpmässig kategori");

            while (true)
            {
                Console.Write("Ange kategorins nummer: ");
                string? input = Console.ReadLine();

                if (input == null)
                    return null;

                if (int.TryParse(input, out int choice))
                {
                    if (choice >= 1 && choice <= categories.Count)
                        return categories[choice - 1];

                    if (choice == randomOption)
                    {
                        string randomCategory = categories[
                            Random.Shared.Next(categories.Count)];

                        Console.WriteLine(
                            $"Slumpmässig kategori: {randomCategory}");

                        return randomCategory;
                    }
                }

                Console.WriteLine("Ogiltigt val. Försök igen.");
            }
        }

        // Läser spelarens svar och frågar om igen om det är fel sorts svar.
        public string ReadAnswer(Question question)
        {
            while (true)
            {
                Console.Write("Ditt svar: ");
                string input = (Console.ReadLine() ?? "").Trim();

                if (question is MultipleChoiceQuestion multipleChoice)
                {
                    if (int.TryParse(input, out int choice) &&
                        choice >= 1 &&
                        choice <= multipleChoice.AnswerOptions.Count)
                    {
                        return input;
                    }

                    Console.WriteLine(
                        $"Ange ett nummer mellan 1 och " +
                        $"{multipleChoice.AnswerOptions.Count}.");
                }
                else if (question is TrueFalseQuestion)
                {
                    switch (input.ToLowerInvariant())
                    {
                        case "t":
                        case "true":
                        case "s":
                        case "sant":
                            return "true";

                        case "f":
                        case "false":
                        case "falskt":
                            return "false";
                    }

                    Console.WriteLine("Svara sant (s) eller falskt (f).");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(input))
                        return input;

                    Console.WriteLine("Svaret får inte vara tomt.");
                }
            }
        }
    }
}