using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Betauer.Core;
using Veronenger.Game.Deck;
using Veronenger.Game.Deck.Hands;
using static System.Int32;


namespace Veronenger.Game;

public class SolitaireConsoleDemo(bool autoPlay, int maxSimulations, float simulationPercentage) {
    public PlayAssistant PlayAssistant = new PlayAssistant(maxSimulations, simulationPercentage);
    public GameRun GameRun;
    public Random GameRunRandom;
    public GameHandler GameHandler;
    public AssistantDecision? CurrentDecision;

    public static void Main() {
        new SolitaireConsoleDemo(false, 1000, 0.2f).ConsoleRun();
    }

    private void ConsoleRun() {
        while (true) {
            Run(0, DisplayGameState, () => {
                IncreaseRandomPokerHandLevel();
                IncreaseRandomPokerHandLevel();
            });
            Console.WriteLine("\nPlay again? (Y/N)");
            if (Console.ReadLine()?.ToUpper() != "Y") {
                break;
            }
        }
    }

    public void Run(int gameId, Action onHand, Action onLevelWon) {
        InitializeRun(gameId);
        while (!GameRun.RunIsGameOver()) {
            GameLevelLoop(onHand);

            if (GameHandler.IsWon() && !GameRun.RunIsWon()) {
                onLevelWon();
                DisplayGameLevelWin();
                InitializeGame(GameHandler.State.Level + 1);
            }
        }

        if (GameRun.RunIsWon()) {
            DisplayFinalVictoryScreen();
        } else {
            DisplayGameOverScreen();
        }
    }

    private void GameLevelLoop(Action onHand) {
        while (!GameHandler.IsGameOver()) {
            CurrentDecision = PlayAssistant.GetNextAction(GameHandler);
            onHand();
            if (autoPlay) {
                ProcessAutoPlay();
                //Thread.Sleep(1000);
            } else {
                ProcessUserInput();
            }
        }
    }

    private void InitializeRun(int seed) {
        var level = 0;
        if (!autoPlay) {
            Console.WriteLine("Enter a base seed number (or press Enter for random seed):");
            var input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) {
                seed = new Random().Next();
            } else if (TryParse(input, out seed)) {
            } else {
                seed = input.GetHashCode(); // Use string hash as seed if not a number
            }
            Console.WriteLine($"Seed: {seed}");

            Console.WriteLine($"Level (press Enter to start from 0):");
            input = Console.ReadLine();
            _ = TryParse(input, out level);
            Console.WriteLine($"Level: {level}");
            Thread.Sleep(600);
        }

        var config = new PokerGameConfig();
        var handsManager = new PokerHandsManager(new Random(seed), new PokerHandConfig());
        handsManager.RegisterBasicPokerHands();
        GameRun = new GameRun(config, handsManager, seed);
        GameRunRandom = new Random(seed);
        InitializeGame(level);
    }

    private void InitializeGame(int level) {
        GameHandler = GameRun.CreateGameHandler(level, DeckBuilder.ClassicPokerDeck());
        GameHandler.DrawCards();
    }

    public void DisplayGameState() {
        Console.Clear();
        Console.WriteLine($"=== {GameRun}");
        DisplayHistory();
        Console.WriteLine($"=== Your hand ===");
        DisplayYourHand();
    }

    public void DisplayHistory() {
        foreach (var gameState in GameRun.GameStates) {
            Console.Write($"  Level {gameState.Level + 1} | ");
            foreach (var action in gameState.History.GetHistory()) {
                if (action.Type == PlayHistory.PlayedActionType.Play)
                    Console.Write($"Play #{action.Id + 1}: {action.PlayedHand?.Name} ({string.Join(", ", action.Cards)}) (Score +{action.HandScore}: {action.GameScore}/{action.TotalScore}) | ");
                else
                    Console.Write($"Discard #{action.Id + 1}: {string.Join(" ", action.Cards)} | ");
            }
            if (gameState.IsWon()) {
                Console.WriteLine("Won!");
            } else if (gameState.IsGameOver()) {
                Console.WriteLine("Game Over!");
            } else {
                Console.WriteLine("...");
            }
        }
    }

    public void IncreaseRandomPokerHandLevel() {
        var randomPokerHand = GameRunRandom.Next(GameRun.PokerHandsManager.ListHandTypes().ToList());
        GameRun.State.SetPokerHandLevel(randomPokerHand, GameRun.State.GetPokerHandLevel(randomPokerHand) + 1);
    }

    private void DisplayGameLevelWin() {
        if (autoPlay) return;
        Console.Clear();
        Console.WriteLine("=== WINNER! ===");
        Console.WriteLine($"Congratulations! You've reached {GameHandler.State.Score} points!");
        Console.WriteLine($"Target was: {GameHandler.State.LevelScore}");

        if (!GameRun.RunIsWon()) {
            Console.WriteLine($"\nAdvancing to level {GameHandler.State.Level + 1}! New target: {GameHandler.State.LevelScore}");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    private void DisplayGameOverScreen() {
        if (autoPlay) return;
        Console.Clear();
        Console.WriteLine("=== GAME OVER ===");
        Console.WriteLine($"Final Score: {GameHandler.State.Score}");
        Console.WriteLine($"Target Score: {GameHandler.State.LevelScore}");
        Console.WriteLine($"You needed {GameHandler.State.LevelScore - GameHandler.State.Score} more points to win");
    }

    private void DisplayFinalVictoryScreen() {
        if (autoPlay) return;
        Console.Clear();
        Console.WriteLine("=== CONGRATULATIONS! ===");
        Console.WriteLine("You've completed all levels!");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private void DisplayYourHand() {
        if (autoPlay) return;
        var cards = GameHandler.State.CurrentHand;
        var groupedCards = cards
            .GroupBy(c => c.Suit)
            .OrderBy(g => g.Key)
            .ToList();

        Console.Write("Your hand by suit: ");
        foreach (var group in groupedCards) {
            var sortedCards = group.OrderByDescending(c => c.Rank);
            Console.Write($"{string.Join(" ", sortedCards)} | ");
        }
        Console.Write("- By rank: ");
        Console.WriteLine($"{string.Join(" ", cards.OrderByDescending(c => c.Rank))}");
    }

    private void DisplayPotentialHands() {
        Console.WriteLine("\nPossible hands you can play:");
        int i = 0;
        for (; i < CurrentDecision.PossibleHands.Count; i++) {
            Console.WriteLine($"{i + 1}: {CurrentDecision.PossibleHands[i].Name}: +{GameHandler.CalculateScore(CurrentDecision.PossibleHands[i])}");
        }
        var currentBestHand = CurrentDecision.PossibleHands[0];

        var bestHandCards = CurrentDecision.PossibleHands[0].Cards;

        foreach (var discards in CurrentDecision.DiscardOptions.Discards) {
            i++;
            var discardBestHand = discards.GetBestHand();

            var breaksYouHand = discards.CardsToDiscard.Any(bestHandCards.Contains);

            Console.WriteLine($"{i}. Discarding {discards.CardsToDiscard.Count}: {string.Join(" ", discards.CardsToDiscard)}, keeping: {string.Join(" ", discards.CardsToKeep)} | Score: {discardBestHand.PotentialScore:F2}{(breaksYouHand ? " | Breaks your current hand!" : "")}");
            var handsByScore = discards.HandOccurrences
                .OrderByDescending(kv => kv.Value.PotentialScore);

            foreach (var (handType, stats) in handsByScore) {
                Console.WriteLine($"   - {handType,-15} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]" : "[ ]")} {(handType == currentBestHand.HandType ? "same as your hand" : "")}");
            }
        }
        Console.WriteLine($"Analysis time: {CurrentDecision.DiscardOptions.ElapsedTime.TotalSeconds:F3} seconds");
        Console.WriteLine($"Total simulations: {CurrentDecision.DiscardOptions.TotalSimulations:N0}/{CurrentDecision.DiscardOptions.TotalCombinations:N0} ({(float)CurrentDecision.DiscardOptions.TotalSimulations / CurrentDecision.DiscardOptions.TotalCombinations:0%})");
    }

    private void ProcessUserInput() {
        DisplayPotentialHands();

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1-N: Play hand/discard from the list");
        Console.WriteLine("M: Make your own hand by selecting cards");
        Console.WriteLine("D: Discard cards");
        Console.WriteLine("A: Auto play: " + GetAutoPlay());
        Console.WriteLine("CTRL+C: Quit game");

        var option = Console.ReadLine()?.ToUpper();

        if (option == "A") {
            ProcessAutoPlay();
        } else if (option == "M") {
            ProcessManualHand();
        } else if (option == "D") {
            if (GameHandler.CanDiscard())
                ProcessManualDiscard();
            else
                Console.WriteLine("No discards remaining!");
        } else if (TryParse(option, out int choice) && choice > 0) {
            if (choice <= CurrentDecision.PossibleHands.Count) {
                var hand = CurrentDecision.PossibleHands[choice - 1];
                ProcessHand(hand.Cards);
            } else {
                choice -= CurrentDecision.PossibleHands.Count;
                var cardsToDiscard = CurrentDecision.DiscardOptions.Discards[choice - 1].CardsToDiscard;
                ProcessDiscard(cardsToDiscard);
            }
        } else {
            Console.WriteLine("Invalid option!");
            Thread.Sleep(300);
        }
    }

    private string GetAutoPlay() {
        if (CurrentDecision == null) throw new InvalidOperationException("currentDecision is null");
        var bestHandIfDiscard = CurrentDecision.DiscardOption?.GetBestHand();
        if (CurrentDecision.HandToPlay != null && CurrentDecision.ShouldPlay) {
            return $"[Play {CurrentDecision.HandToPlay}] +{GameHandler.CalculateScore(CurrentDecision.HandToPlay)} | Reason: {CurrentDecision.Reason}";
        } else {
            return $"[Discard {string.Join(", ", CurrentDecision.DiscardOption!.CardsToDiscard)}] score {bestHandIfDiscard!.AvgScore:0} x {bestHandIfDiscard!.Probability:0.0%} = {bestHandIfDiscard!.PotentialScore:0.0} | Reason: {CurrentDecision.Reason}";
        }
    }

    private void ProcessAutoPlay() {
        if (CurrentDecision == null) throw new InvalidOperationException("currentDecision is null");
        if (CurrentDecision.HandToPlay != null && CurrentDecision.ShouldPlay) {
            var extraCards = GameHandler.Config.MaxHandSizeToPlay - CurrentDecision.HandToPlay.Cards.Count;
            if (extraCards > 0) {
                // If the current hand to play is smaller than the max size, add extra cards just to use them as discard
                var noPlayedCards = GameHandler.State.CurrentHand.Except(CurrentDecision.HandToPlay.Cards);
                var extraCardsToDiscard = noPlayedCards.OrderBy(i => i.Rank).Take(extraCards).ToList();
                IReadOnlyList<Card> newPokerHand = [..CurrentDecision.HandToPlay.Cards, ..extraCardsToDiscard];
                ProcessHand(newPokerHand);
            } else {
                ProcessHand(CurrentDecision.HandToPlay.Cards);
            }
        } else {
            ProcessDiscard(CurrentDecision.DiscardOption!.CardsToDiscard);
        }
    }

    private void ProcessManualHand() {
        while (true) {
            var state = GameHandler.State;
            Console.WriteLine("\nSelect cards to play (enter card positions, e.g., '1 3 5'):");
            Console.WriteLine("Current hand:");
            for (int i = 0; i < state.CurrentHand.Count; i++) {
                Console.WriteLine($"{i + 1}: {state.CurrentHand[i]}");
            }
            Console.WriteLine("C: Cancel");

            var input = Console.ReadLine();
            if (input?.ToUpper() == "C") {
                break;
            }
            var selectedIndices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => TryParse(s, out int n) ? n - 1 : -1)
                .Where(n => n >= 0 && n < state.CurrentHand.Count)
                .ToList();

            if (selectedIndices == null || selectedIndices.Count == 0) {
                Console.WriteLine("Invalid selection!");
                continue;
            }

            var selectedCards = selectedIndices.Select(i => state.CurrentHand[i]).ToList();
            var possibleHands = GameHandler.PokerHandsManager.IdentifyAllHands(GameHandler, selectedCards);

            if (possibleHands.Count == 0) {
                Console.WriteLine("No valid poker hand can be formed with these cards!");
                continue;
            }

            var hand = possibleHands[0];
            ProcessHand(hand.Cards);
            break;
        }
    }

    private void ProcessHand(IReadOnlyList<Card> hand) {
        var result = GameHandler.PlayHand(hand);

        if (!autoPlay) {
            Console.WriteLine($"Played {result.Hand?.Name}: {string.Join(", ", hand)}. Scored: +{result.Score} ({GameHandler.State.Score}/{GameHandler.State.LevelScore})");
        }

        if (GameHandler.IsDrawPending()) {
            GameHandler.DrawCards();
            DisplayYourHand();
        }
        if (!autoPlay) {
            Console.WriteLine("(press any key to continue)");
            Console.ReadKey();
        }
    }

    private void ProcessManualDiscard() {
        while (true) {
            var state = GameHandler.State;
            Console.WriteLine("\nEnter the indices of cards to discard (1-7, separated by spaces):");
            Console.WriteLine("Current hand:");
            for (int i = 0; i < state.CurrentHand.Count; i++) {
                Console.WriteLine($"{i + 1}: {state.CurrentHand[i]}");
            }
            Console.WriteLine("C: Cancel");

            var input = Console.ReadLine();
            if (input?.ToUpper() == "C") {
                break;
            }

            var indices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => TryParse(s, out int n) ? n - 1 : -1)
                .Where(n => n >= 0 && n < state.CurrentHand.Count)
                .ToList();

            if (indices == null || indices.Count == 0 || indices.Count > GameHandler.Config.MaxDiscardCards) {
                Console.WriteLine($"Error. Please select between 1 and {GameHandler.Config.MaxDiscardCards} valid cards!");
                continue;
            }

            var cardsToDiscard = indices.Select(i => state.CurrentHand[i]).ToList();
            ProcessDiscard(cardsToDiscard);
            break;
        }
    }

    private void ProcessDiscard(List<Card> cardsToDiscard) {
        var result = GameHandler.Discard(cardsToDiscard);
        if (!autoPlay) {
            Console.WriteLine($"Discarded: {string.Join(", ", cardsToDiscard)}");
            Console.WriteLine($"* Cards discarded. Remaining discards: {GameHandler.Config.MaxDiscards - GameHandler.State.Discards}");
        }
        GameHandler.DrawCards();
        DisplayYourHand();
        if (!autoPlay) {
            Console.WriteLine("(press any key to continue)");
            Console.ReadKey();
        }
    }

    private const float Risk = 0.7f;
}