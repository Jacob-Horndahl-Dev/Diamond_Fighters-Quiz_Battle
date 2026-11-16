
using System;
using System.Collections.Generic;
using System.Linq;

namespace Diamond_Fighters_Quiz_Battle
{
    internal class Game
    {
        private readonly List<Player> players;
        private readonly List<Question> questions;
        private readonly ScoreManager scoreManager;
        private readonly HighScoreManager highScores;
        private readonly int totalRounds;

        public Game(List<Player> players, List<Question> questions, int totalRounds, HighScoreManager highScores)
        {
            if (players == null || players.Count < 2 || players.Count > 4)
                throw new ArgumentException("Spelet kräver 2–4 spelare.");

            if (players.Any(p => p == null ||
                                 string.IsNullOrWhiteSpace(p.Name)) ||
                players.Select(p => p.Name.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count() != players.Count)
            {
                throw new ArgumentException(
                    "Alla spelare måste ha olika namn och namnen får inte vara tomma.");
            }

            if (questions == null)
                throw new ArgumentNullException(nameof(questions));

            if (totalRounds < 1 || totalRounds > 10)
                throw new ArgumentException("Välj mellan 1 och 10 rundor.");

            this.players = players;
            this.questions = new List<Question>(questions);
            this.totalRounds = totalRounds;
            scoreManager = new ScoreManager();
            this.highScores = highScores;
        }

        public void StartGame()
        {
            Console.WriteLine("Välkommen till Quiz Battle!");

            foreach (Player player in players)
            {
                player.Score = 0;
                player.ResetStreak();
                player.CorrectAnswers = 0;
            }

            for (int round = 1; round <= totalRounds; round++)
            {
                if (questions.Count == 0)
                {
                    Console.WriteLine("Det finns inga fler frågor.");
                    break;
                }

                Console.WriteLine($"\n=== Runda {round} av {totalRounds} ===");
                PlayRound();
            }

            EndGame();
        }

        public void PlayRound()
        {
            foreach (Player player in players)
            {
                if (questions.Count == 0)
                {
                    Console.WriteLine("Det finns inga fler frågor.");
                    return;
                }

                Console.WriteLine($"\nNu är det {player.Name}:s tur!");

                string? category = ChooseCategory();

                if (category == null)
                    return;

                List<Question> availableQuestions = questions
                    .Where(q => string.Equals(
                        q.Category,
                        category,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (availableQuestions.Count == 0)
                {
                    Console.WriteLine("Det finns inga frågor tillgängliga.");
                    return;
                }

                // Välj en slumpmässig fråga som inte använts tidigare.
                Question question = availableQuestions[
                    Random.Shared.Next(availableQuestions.Count)];

                // Ta bort frågan så att den inte kommer igen.
                questions.Remove(question);

                int difficulty = GetDifficulty(question.DifficultyLevel);
                int basePoints = question.GetScore();

                Console.WriteLine(
                    $"\nKategori: {question.Category} ({basePoints} poäng)");

                Console.WriteLine(question.QuestionText);

                question.ShowQuestionAnswerOptions();

                string answer = ReadAnswer(question);

                if (question.CheckAnswer(answer))
                {
                    int points = scoreManager.CalculateScore(
                        difficulty,
                        player.Streak);

                    int bonus = points - basePoints;

                    player.AddPoints(points);
                    player.Streak++;
                    player.CorrectAnswers++;

                    Console.WriteLine($"\nRätt svar! +{points} poäng.");

                    if (bonus > 0)
                    {
                        Console.WriteLine(
                            $"Bonus för rätt svar i rad: +{bonus} poäng " +
                            $"({player.Streak} rätt i rad).");
                    }
                }
                else
                {
                    player.ResetStreak();

                    Console.WriteLine("\nFel svar! 0 poäng.");
                    Console.WriteLine(
                        $"Rätt svar: {GetCorrectAnswer(question)}");
                }

                Console.WriteLine($"Totalpoäng: {player.Score}");
            }
        }

        private string? ChooseCategory()
        {
            List<string> categories = questions
                .Select(q => q.Category)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
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

                int remaining = questions.Count(q =>
                    string.Equals(
                        q.Category,
                        category,
                        StringComparison.OrdinalIgnoreCase));

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
                        string randomCategory = questions[
                            Random.Shared.Next(questions.Count)].Category;

                        Console.WriteLine(
                            $"Slumpmässig kategori: {randomCategory}");

                        return randomCategory;
                    }
                }

                Console.WriteLine("Ogiltigt val. Försök igen.");
            }
        }

        private string ReadAnswer(Question question)
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

        private int GetDifficulty(string? difficulty)
        {
            return difficulty?.Trim().ToLowerInvariant() switch
            {
                "easy" => 1,
                "medium" => 2,
                "hard" => 3,
                _ => 1
            };
        }

        private string GetCorrectAnswer(Question question)
        {
            if (question is MultipleChoiceQuestion multipleChoice &&
                int.TryParse(question.CorrectAnswer, out int index) &&
                index >= 0 &&
                index < multipleChoice.AnswerOptions.Count)
            {
                return multipleChoice.AnswerOptions[index];
            }

            return question.CorrectAnswer;
        }

        public void EndGame()
        {
            Console.WriteLine("\n=== Spelet är slut! ===");
            Console.WriteLine("\nSlutresultat:");

            List<Player> rankedPlayers = players
                .OrderByDescending(p => p.Score)
                .ThenByDescending(p => p.CorrectAnswers)
                .ToList();

            for (int i = 0; i < rankedPlayers.Count; i++)
            {
                Player player = rankedPlayers[i];

                Console.WriteLine(
                    $"{i + 1}. {player.Name}: {player.Score} poäng " +
                    $"({player.CorrectAnswers} rätt)");
            }

            List<Player> winners = scoreManager.DetermineWinner(players);

            if (winners.Count == 1)
            {
                Console.WriteLine($"\nVinnare: {winners[0].Name}!");
            }
            else if (winners.Count > 1)
            {
                Console.WriteLine("\nDet blev oavgjort mellan:");

                foreach (Player winner in winners)
                {
                    Console.WriteLine(winner.Name);
                }
            }

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
    }
}
