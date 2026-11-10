
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
            }

            for (int round = 1; round <= totalRounds; round++)
            {
                if (questions.Count == 0)
                {
                    Console.WriteLine("No more questions available.");
                    break;
                }

                Console.WriteLine($"\nRound {round} of {totalRounds}");
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

                Question? question = questions
                    .FirstOrDefault(q =>
                        string.Equals(
                            q.Category,
                            category,
                            StringComparison.OrdinalIgnoreCase));

                if (question == null)
                    return;

                questions.Remove(question);

                Console.WriteLine($"Category: {question.Category}");
                Console.WriteLine($"Difficulty: {question.DifficultyLevel}");
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
                    Console.WriteLine("Answer true or false.");
                }

                Console.Write("Your answer: ");
                string answer = Console.ReadLine() ?? "";

                if (question.CheckAnswer(answer))
                {
                    int difficulty = question.DifficultyLevel?
                        .Trim()
                        .ToLower() switch
                    {
                        "easy" => 1,
                        "medium" => 2,
                        "hard" => 3,
                        _ => 1
                    };

                    int points = scoreManager.CalculateScore(
                        difficulty,
                        player.Streak);

                    player.AddPoints(points);
                    player.Streak++;

                    Console.WriteLine($"Correct! +{points} points.");
                    Console.WriteLine($"Current streak: {player.Streak}");
                }
                else
                {
                    player.ResetStreak();

                    Console.WriteLine("Incorrect! 0 points.");

                    string correctAnswer = question.CorrectAnswer;

                    if (question is MultipleChoiceQuestion mc &&
                        int.TryParse(question.CorrectAnswer, out int index) &&
                        index >= 0 &&
                        index < mc.AnswerOptions.Count)
                    {
                        correctAnswer = mc.AnswerOptions[index];
                    }

                    Console.WriteLine($"Correct answer: {correctAnswer}");
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
                Console.WriteLine($"{i + 1}. {categories[i]}");
            }

            while (true)
            {
                Console.Write("Enter category number: ");
                string input = Console.ReadLine() ?? "";

                if (int.TryParse(input, out int choice) &&
                    choice >= 1 &&
                    choice <= categories.Count)
                {
                    return categories[choice - 1];
                }

                Console.WriteLine("Invalid choice. Try again.");
            }
        }

        public void EndGame()
        {
            Console.WriteLine("\nGame over!");
            Console.WriteLine("\nFinal scores:");

            foreach (Player player in players.OrderByDescending(p => p.Score))
            {
                Console.WriteLine($"{player.Name}: {player.Score} points");
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
