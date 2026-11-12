using System.Text.Json;

namespace Diamond_Fighters_Quiz_Battle
{
    internal class HighScoreManager
    {
        // Sökvägen till JSON-filen där topplistan sparas.
        private readonly string filePath;

        // Sparar sökvägen som skickas in när managern skapas.
        public HighScoreManager(string filePath)
        {
            this.filePath = filePath;
        }

        // Läser filen och ger tillbaka alla sparade rader. Saknas filen blir listan tom.
        public List<HighScoreEntry> LoadHighScores()
        {
            if (!File.Exists(filePath))
            {
                return new List<HighScoreEntry>();
            }

            string json = File.ReadAllText(filePath);

            if (json.Trim() == "")
            {
                return new List<HighScoreEntry>();
            }

            try
            {
                List<HighScoreEntry>? loaded = JsonSerializer.Deserialize<List<HighScoreEntry>>(json);

                if (loaded == null)
                {
                    return new List<HighScoreEntry>();
                }

                return loaded;
            }
            catch (JsonException)
            {
                Console.WriteLine("Topplistan kunde inte läsas.");
                return new List<HighScoreEntry>();
            }
        }

        // Lägger till en rad och skriver tillbaka hela listan, så gamla resultat blir kvar.
        public void SaveHighScore(HighScoreEntry entry)
        {
            List<HighScoreEntry> entries = LoadHighScores();
            entries.Add(entry);

            Directory.CreateDirectory("data");

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;

            string json = JsonSerializer.Serialize(entries, options);
            File.WriteAllText(filePath, json);
        }

        // Skriver ut de tio bästa. Högst poäng först, och vid lika poäng flest rätt först.
        public void ShowHighScores()
        {
            List<HighScoreEntry> entries = LoadHighScores();
            entries.Sort(CompareEntries);

            Console.WriteLine("=== Topplista ===");

            if (entries.Count == 0)
            {
                Console.WriteLine("Topplistan är tom.");
                return;
            }

            // howMany är ett tal: hur många rader som ska skrivas ut. Högst tio.
            int howMany = entries.Count;
            if (howMany > 10)
            {
                howMany = 10;
            }

            for (int i = 0; i < howMany; i++)
            {
                HighScoreEntry entry = entries[i];
                int place = i + 1;

                Console.WriteLine(
                    place + ". " + entry.Name + "  " + entry.Score + " poäng  " + entry.CorrectAnswers + " rätt  " + entry.Date);
            }
        }

        // Jämför två rader åt Sort. Högre poäng hamnar först, sedan flest rätt.
        private int CompareEntries(HighScoreEntry a, HighScoreEntry b)
        {
            if (a.Score != b.Score)
            {
                if (a.Score > b.Score)
                {
                    return -1;
                }

                return 1;
            }
            
            // Lika poäng: den med flest rätt ska stå först.
            if (a.CorrectAnswers != b.CorrectAnswers)
            {
                if (a.CorrectAnswers > b.CorrectAnswers)
                {
                    return -1;
                }

                return 1;
            }

            return 0;
        }
    }
}