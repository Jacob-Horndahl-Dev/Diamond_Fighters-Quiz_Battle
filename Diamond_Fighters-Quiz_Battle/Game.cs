
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
        private readonly int totalRounds;

        public Game(List<Player> players, List<Question> questions, int totalRounds)
        {
            if (players == null || players.Count < 2 || players.Count > 4)
                throw new ArgumentException("The game requires 2-4 players.");

            if (players.Any(p => p == null ||
                                 string.IsNullOrWhiteSpace(p.Name)) ||
                players.Select(p => p.Name.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count() != players.Count)
            {
                throw new ArgumentException(
                    "Players must have unique, non-empty names.");
            }

            if (questions == null)
                throw new ArgumentNullException(nameof(questions));

            if (totalRounds < 1 || totalRounds > 10)
                throw new ArgumentException("Choose 1-10 rounds.");

            this.players = players;
            this.questions = new List<Question>(questions);
            this.totalRounds = totalRounds;
            scoreManager = new ScoreManager();
        }

        public void StartGame()
        {
            Console.WriteLine("Welcome to Quiz Battle!");

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
                    Console.WriteLine("No more questions available.");
                    break;
                }

                Console.WriteLine($"\n=== Round {round} of {totalRounds} ===");
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
                    Console.WriteLine("No more questions available.");
                    return;
                }

                Console.WriteLine($"\n{player.Name}'s turn!");

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
                    Console.WriteLine("No questions available.");
                    return;
                }

                // Select a random unused question.
                Question question = availableQuestions[
                    Random.Shared.Next(availableQuestions.Count)];

                // Remove it so it cannot appear again.
                questions.Remove(question);

                int difficulty = GetDifficulty(question.DifficultyLevel);
                int basePoints = question.GetScore();

                Console.WriteLine(
                    $"\nCategory: {question.Category} ({basePoints} points)");

                Console.WriteLine(question.QuestionText);

                if (question is MultipleChoiceQuestion multipleChoice)
                {
                    for (int i = 0; i < multipleChoice.AnswerOptions.Count; i++)
                    {
                        Console.WriteLine(
                            $"{i + 1}. {multipleChoice.AnswerOptions[i]}");
                    }
                }
                else if (question is TrueFalseQuestion)
                {
                    Console.WriteLine("True (t) or false (f)?");
                }

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

                    Console.WriteLine($"\nCorrect! +{points} points.");

                    if (bonus > 0)
                    {
                        Console.WriteLine(
                            $"Streak bonus: +{bonus} points " +
                            $"({player.Streak} correct in a row).");
                    }
                }
                else
                {
                    player.ResetStreak();

                    Console.WriteLine("\nIncorrect! 0 points.");
                    Console.WriteLine(
                        $"Correct answer: {GetCorrectAnswer(question)}");
                }

                Console.WriteLine($"Total score: {player.Score}");
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
                Console.WriteLine("No categories available.");
                return null;
            }

            Console.WriteLine("\nChoose a category:");

            for (int i = 0; i < categories.Count; i++)
            {
                string category = categories[i];

                int remaining = questions.Count(q =>
                    string.Equals(
                        q.Category,
                        category,
                        StringComparison.OrdinalIgnoreCase));

                Console.WriteLine(
                    $"{i + 1}. {category} ({remaining} questions left)");
            }

            int randomOption = categories.Count + 1;

            Console.WriteLine($"{randomOption}. Random category");

            while (true)
            {
                Console.Write("Enter category number: ");
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
                            $"Random category: {randomCategory}");

                        return randomCategory;
                    }
                }

                Console.WriteLine("Invalid choice. Try again.");
            }
        }

        private string ReadAnswer(Question question)
        {
            while (true)
            {
                Console.Write("Your answer: ");
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
                        $"Enter a number between 1 and " +
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

                    Console.WriteLine("Enter true (t) or false (f).");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(input))
                        return input;

                    Console.WriteLine("Answer cannot be empty.");
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
            Console.WriteLine("\n=== Game over! ===");
            Console.WriteLine("\nFinal scores:");

            List<Player> rankedPlayers = players
                .OrderByDescending(p => p.Score)
                .ThenByDescending(p => p.CorrectAnswers)
                .ToList();

            for (int i = 0; i < rankedPlayers.Count; i++)
            {
                Player player = rankedPlayers[i];

                Console.WriteLine(
                    $"{i + 1}. {player.Name}: {player.Score} points " +
                    $"({player.CorrectAnswers} correct)");
            }

            List<Player> winners = scoreManager.DetermineWinner(players);

            if (winners.Count == 1)
            {
                Console.WriteLine($"\nWinner: {winners[0].Name}!");
            }
            else if (winners.Count > 1)
            {
                Console.WriteLine("\nIt's a tie between:");

                foreach (Player winner in winners)
                {
                    Console.WriteLine(winner.Name);
                }
            }
        }
    }
}
