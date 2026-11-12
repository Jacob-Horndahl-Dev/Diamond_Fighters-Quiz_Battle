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
                Console.WriteLine("⚔  Quiz Battle  ⚔");
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
                        StartGame();
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
            Console.Write("[ PRESS ENTER TO START ]");
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

        // Läser antal spelare, namn och rundor. Spelet kopplas in när Game finns.
        public void StartGame()
        {
            int playerCount = GetPlayerInput("Antal spelare (2-4): ", 2, 4);
            List<string> names = ReadPlayerNames(playerCount);
            int rounds = GetPlayerInput("Antal rundor (1-10): ", 1, 10);

            Console.WriteLine("Spelare: " + string.Join(", ", names));
            Console.WriteLine("Rundor: " + rounds);
            Console.WriteLine("Spelet kopplas in när Game är mergad.");
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
    }
}