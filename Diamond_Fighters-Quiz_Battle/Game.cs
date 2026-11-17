using System;
using System.Collections.Generic;
using System.Linq;

namespace Diamond_Fighters_Quiz_Battle
{
    internal class Game
    {
        private readonly List<Player> players;
        private readonly QuestionManager questionManager;
        private readonly ScoreManager scoreManager;
        private readonly Menu menu;
        private readonly int totalRounds;

        public Game(
            List<Player> players,
            QuestionManager questionManager,
            int totalRounds,
            Menu menu)
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

            if (questionManager == null)
                throw new ArgumentNullException(nameof(questionManager));

            if (menu == null)
                throw new ArgumentNullException(nameof(menu));

            if (totalRounds < 1 || totalRounds > 10)
                throw new ArgumentException("Välj mellan 1 och 10 rundor.");

            this.players = players;
            this.questionManager = questionManager;
            this.totalRounds = totalRounds;
            scoreManager = new ScoreManager();
            this.menu = menu;
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
                if (questionManager.OutOfQuestions)
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
                if (questionManager.OutOfQuestions)
                {
                    Console.WriteLine("Det finns inga fler frågor.");
                    return;
                }

                Console.WriteLine($"\nNu är det {player.Name}:s tur!");

                string? category = menu.ChooseCategory(questionManager);

                if (category == null)
                    return;

                Question? question = questionManager.GetQuestion(category);

                if (question == null)
                {
                    Console.WriteLine("Det finns inga frågor tillgängliga.");
                    return;
                }
                                
                int basePoints = question.GetScore();

                Console.WriteLine(
                    $"\nKategori: {question.Category} ({basePoints} poäng)");

                Console.WriteLine(question.QuestionText);
                question.ShowQuestionAnswerOptions();
                string answer = menu.ReadAnswer(question);

                if (question.CheckAnswer(answer))
                {
                    int points = scoreManager.CalculateScore(basePoints, player.Streak);
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

        private string GetCorrectAnswer(Question question)
        {
            if (question is MultipleChoiceQuestion multipleChoice &&
                int.TryParse(question.CorrectAnswer, out int index) &&
                index >= 0 &&
                index < multipleChoice.AnswerOptions.Count)
            {
                return multipleChoice.AnswerOptions[index];
            }

            if (question is TrueFalseQuestion)
            {
                bool isTrue = question.CorrectAnswer
                    .Equals("true", StringComparison.OrdinalIgnoreCase);
                return isTrue ? "sant" : "falskt";
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
        }
    }
}