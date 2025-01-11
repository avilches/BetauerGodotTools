using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Examples;

public class GameRun {
    public DateTime StartTime { get; }
    public List<GameState> GameStates { get; }

    public GameRun() {
        StartTime = DateTime.Now;
        GameStates = new List<GameState>();
    }

    public void AddGameState(GameState state) {
        GameStates.Add(state);
    }

    public override string ToString() {
        var lastState = GameStates.LastOrDefault();
        return lastState != null
            ? $"{StartTime:yyyy-MM-dd HH:mm:ss} - Max level {lastState.Level + 1} - Score {lastState.Score}/{lastState.TotalScore}"
            : $"{StartTime:yyyy-MM-dd HH:mm:ss} - No games played";
    }
}

public class SolitaireConsoleDemo {
    private readonly Random random;
    private readonly int[] scoreThresholds = { 300, 500, 800, 1200, 2000 };
    private bool autoPlay;
    private readonly List<GameRun> gameRuns = new();
    private GameRun currentRun;
    public SolitairePokerGame Game;

    public SolitaireConsoleDemo(Random random, bool autoPlay = false) {
        this.random = random;
        this.autoPlay = autoPlay;
    }

    public static void Main() {
        Console.WriteLine("Auto-play mode? (Y/N)");
        var autoPlay = Console.ReadLine()?.ToUpper() == "Y";
        new SolitaireConsoleDemo(new Random(1), autoPlay).Play();
    }

    public void Play() {
        InitializeCurrentRun();

        while (Game.State.Level < scoreThresholds.Length) {
            while (!Game.IsGameOver()) {
                DisplayGameState();

                if (autoPlay) {
                    var possibleHands = Game.GetPossibleHands();
                    var discardOptions = Game.GetDiscardOptions(possibleHands[0].Cards);
                    ProcessAutoPlay(possibleHands, discardOptions);
                    // System.Threading.Thread.Sleep(1000);
                } else {
                    if (ProcessUserInput() == true) {
                        return; // El usuario quiere salir
                    }
                }
            }

            // Guardamos el estado del juego actual
            currentRun.AddGameState(Game.State);

            if (Game.IsWon()) {
                DisplayWinScreen();

                if (Game.State.Level + 1 < scoreThresholds.Length) {
                    // Preparamos el siguiente nivel
                    InitializeGame(Game.State.Level + 1);

                    Console.WriteLine($"\nAdvancing to level {Game.State.Level + 1}! New target: {Game.State.TotalScore}");
                    if (!autoPlay) {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
            } else {
                // El juego terminó sin alcanzar la puntuación
                DisplayGameOverScreen();
                if (!autoPlay && !PlayAgain()) {
                    return;
                }
                InitializeCurrentRun();
            }
        }

        DisplayFinalVictoryScreen();
    }

    private void InitializeCurrentRun() {
        currentRun = new GameRun();
        gameRuns.Add(currentRun);
        InitializeGame(0);
    }

    private void InitializeGame(int level) {
        Game = new SolitairePokerGame(random, new PokerGameConfig());
        Game.Hands.RegisterBasicPokerHands();
        Game.DrawCards();
        Game.State.Level = level;
        Game.State.TotalScore = scoreThresholds[level];
    }

    private void DisplayGameState() {
        var state = Game.State;
        Console.Clear();

        // Display runs history
        if (gameRuns.Count > 0) {
            Console.WriteLine("=== Previous Runs ===");
            // foreach (var run in gameRuns.OrderByDescending(r => r.StartTime)) {
            foreach (var run in gameRuns) {
                Console.WriteLine(run);
                foreach (var gameState in run.GameStates) {
                    Console.Write($"  Level {gameState.Level + 1}:");
                    foreach (var action in gameState.History.GetHistory()) {
                        if (action.Type == GameHistory.GameActionType.Play)
                            Console.Write($"Play #{action.Id + 1}: {action.PlayedHand} (Score +{action.HandScore}: {action.GameScore}/{action.TotalScore}) | ");
                        else
                            Console.Write($"Discard #{action.Id + 1}: {string.Join(" ", action.Cards)} | ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        Console.WriteLine($"=== Solitaire Poker - Level {state.Level + 1} ===");
        Console.WriteLine($"Score: {state.Score}/{state.TotalScore}");
        Console.WriteLine($"Hand {state.HandsPlayed + 1}/{Game.Config.MaxHands}");
        Console.WriteLine($"Discards: {state.Discards}/{Game.Config.MaxDiscards}");
        Console.WriteLine("\nYour hand:");
        DisplayCards(state.CurrentHand);
        Console.WriteLine("\nHistory: ");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == GameHistory.GameActionType.Play)
                Console.Write($"Play #{action.Id + 1}: {action.PlayedHand} (Score +{action.HandScore}: {action.GameScore}/{action.TotalScore}) | ");
            else
                Console.Write($"Discard #{action.Id + 1}: {string.Join(" ", action.Cards)} | ");
        }
        Console.WriteLine();
    }

    private void DisplayWinScreen() {
        Console.Clear();
        Console.WriteLine("=== WINNER! ===");
        Console.WriteLine($"Congratulations! You've reached {Game.State.Score} points!");
        Console.WriteLine($"Target was: {Game.State.TotalScore}");
    }

    private void DisplayGameOverScreen() {
        Console.Clear();
        Console.WriteLine("=== GAME OVER ===");
        Console.WriteLine($"Final Score: {Game.State.Score}");
        Console.WriteLine($"Target Score: {Game.State.TotalScore}");
        Console.WriteLine($"You needed {Game.State.TotalScore - Game.State.Score} more points to win");
    }

    private void DisplayFinalVictoryScreen() {
        Console.Clear();
        Console.WriteLine("=== CONGRATULATIONS! ===");
        Console.WriteLine("You've completed all levels!");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private bool PlayAgain() {
        if (autoPlay) return true;
        Console.WriteLine("\nPlay again? (Y/N)");
        return Console.ReadLine()?.ToUpper() == "Y";
    }

    private void DisplayCards(IReadOnlyList<Card> cards) {
        var groupedCards = cards
            .GroupBy(c => c.Suit)
            .OrderBy(g => g.Key)
            .ToList();

        Console.Write("By suit: ");
        foreach (var group in groupedCards) {
            var sortedCards = group.OrderByDescending(c => c.Rank);
            Console.Write($"{string.Join(" ", sortedCards)} | ");
        }
        Console.WriteLine();
        Console.Write("By rank: ");
        Console.WriteLine($"{string.Join(" ", cards.OrderByDescending(c => c.Rank))}");
    }

    private void DisplayPotentialHands(int i, PokerHand currentBestHand, DiscardOptionsResult discardOptions) {
        var options = discardOptions.GetBestDiscards(0);

        Console.WriteLine("\nPotential discard options:");
        Console.WriteLine($"Analysis time: {discardOptions.ElapsedTime.TotalSeconds:F3} seconds");
        Console.WriteLine($"Total simulations: {discardOptions.TotalSimulations:N0}/{discardOptions.TotalCombinations:N0} ({(float)discardOptions.TotalSimulations / discardOptions.TotalCombinations:0%})");

        foreach (var option in options.Where(option => option.GetBestPotentialScore(Risk) > Risk)) {
            var discardBestHand = option.GetBestHand(Risk)!;
            Console.WriteLine($"\n{i}. Discarding {option.CardsToDiscard.Count}: {string.Join(" ", option.CardsToDiscard)}, keeping: {string.Join(" ", option.CardsToKeep)}  Score: {discardBestHand.PotentialScore:F2}");
            var handsByScore = option.HandOccurrences
                .OrderByDescending(kv => kv.Value.PotentialScore);

            foreach (var (handType, stats) in handsByScore) {
                Console.WriteLine($"   - {handType.Name,-20} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]" : "[ ]")} {(handType == currentBestHand.GetType() ? "redundant" : "")}");
            }
            i++;
        }
        foreach (var option in options.Where(option => option.GetBestPotentialScore(Risk) <= Risk)) {
            var discardBestHand = option.GetBestHand(0)!;
            Console.WriteLine($"\n{i}. Discarding {option.CardsToDiscard.Count}: {string.Join(" ", option.CardsToDiscard)}, keeping: {string.Join(" ", option.CardsToKeep)}  Score: {discardBestHand.PotentialScore:F2}");
            var handsByScore = option.HandOccurrences
                .OrderByDescending(kv => kv.Value.PotentialScore);

            foreach (var (handType, stats) in handsByScore) {
                Console.WriteLine($"   - {handType.Name,-20} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]" : "[ ]")} {(handType == currentBestHand.GetType() ? "redundant" : "")}");
            }
            i++;
        }
    }

    private void DisplayPossibleHands(List<PokerHand> hands) {
        Console.WriteLine("\nPossible hands you can play:");
        for (int i = 0; i < hands.Count; i++) {
            Console.WriteLine($"{i + 1}: {hands[i]}: " + hands[i].CalculateScore());
        }
    }

    private bool ProcessUserInput() {
        var possibleHands = Game.GetPossibleHands();

        // Autoplay: los descartes nunca se descartan de las cartas de la mejor mano actual
        var discardOptions = Game.GetDiscardOptions(possibleHands[0].Cards);
        DisplayPossibleHands(possibleHands);
        DisplayPotentialHands(possibleHands.Count + 1, possibleHands[0], discardOptions);

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1-N: Play hand/discard from the list");
        Console.WriteLine("M: Make your own hand by selecting cards");
        Console.WriteLine("D: Discard cards");
        Console.WriteLine("A: Auto play: " + GetAutoPlay(possibleHands, discardOptions));
        Console.WriteLine("Q: Quit game");

        var option = Console.ReadLine()?.ToUpper();

        if (option == "Q") {
            return true;
        }

        if (option == "A") {
            ProcessAutoPlay(possibleHands, discardOptions);
        } else if (option == "M") {
            ProcessManualHand();
        } else if (option == "D") {
            if (Game.CanDiscard())
                ProcessManualDiscard();
            else
                Console.WriteLine("No discards remaining!");
        } else if (int.TryParse(option, out int choice) && choice > 0) {
            if (choice <= possibleHands.Count) {
                var hand = possibleHands[choice - 1];
                ProcessHand(hand);
            } else {
                choice -= possibleHands.Count;
                var cardsToDiscard = discardOptions.DiscardOptions[choice - 1].CardsToDiscard;
                ProcessDiscard(cardsToDiscard);
            }
        } else {
            Console.WriteLine("Invalid option! (press any key to continue)");
            Console.ReadKey();
        }
        return false;
    }

    private string GetAutoPlay(List<PokerHand> possibleHands, DiscardOptionsResult discardOptions) {
        // Si no podemos descartarnos más, jugamos la mejor mano disponible
        if (!Game.CanDiscard()) {
            return "No more discards. Play best hand " + possibleHands[0];
        }

        var bestHand = possibleHands[0];
        var bestDiscardOption = discardOptions.GetBestDiscards(Risk).FirstOrDefault();

        var bestDiscardHandScore = bestDiscardOption?.GetBestPotentialScore(Risk) ?? 0;
        if (bestHand.CalculateScore() >= bestDiscardHandScore) {
            return $"Best hand ({possibleHands[0]} scores {bestHand.CalculateScore():0} >= potential discard ({string.Join(", ", bestDiscardOption?.CardsToDiscard ?? [])}) score {bestDiscardHandScore:0})";
        } else {
            return $"Best discard ({possibleHands[0]} scores {bestHand.CalculateScore():0} < potential discard ({string.Join(", ", bestDiscardOption?.CardsToDiscard ?? [])}) score {bestDiscardHandScore:0})";
        }
    }

    private void ProcessAutoPlay(List<PokerHand> possibleHands, DiscardOptionsResult discardOptions) {
        // Si no podemos descartarnos más, jugamos la mejor mano disponible
        if (!Game.CanDiscard()) {
            ProcessHand(possibleHands[0]);
            return;
        }

        var bestHand = possibleHands[0];
        var bestDiscardOption = discardOptions.GetBestDiscards(Risk).FirstOrDefault();

        var bestDiscardHandScore = bestDiscardOption?.GetBestPotentialScore(Risk) ?? 0;
        if (bestHand.CalculateScore() >= bestDiscardHandScore) {
            ProcessHand(bestHand);
        } else {
            ProcessDiscard(bestDiscardOption!.CardsToDiscard);
        }
    }

    private void ProcessManualHand() {
        while (true) {
            var state = Game.State;
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
                .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
                .Where(n => n >= 0 && n < state.CurrentHand.Count)
                .ToList();

            if (selectedIndices == null || selectedIndices.Count == 0) {
                Console.WriteLine("Invalid selection!");
                continue;
            }

            var selectedCards = selectedIndices.Select(i => state.CurrentHand[i]).ToList();
            var possibleHands = Game.Hands.IdentifyAllHands(selectedCards);

            if (possibleHands.Count == 0) {
                Console.WriteLine("No valid poker hand can be formed with these cards!");
                continue;
            }

            var hand = possibleHands[0];
            ProcessHand(hand);
            break;
        }
    }

    private void ProcessHand(PokerHand hand) {
        var result = Game.PlayHand(hand);

        Console.WriteLine($"Played {hand.GetType().Name}: {string.Join(", ", hand)}. Scored: +{result.Score} ({Game.State.Score}/{Game.State.TotalScore})");

        if (Game.DrawPending()) {
            Game.DrawCards();
            Console.WriteLine($"* New hand: {string.Join(", ", Game.State.CurrentHand)}");
        }
        if (!autoPlay) {
            Console.WriteLine("(press any key to continue)");
            Console.ReadKey();
        }
    }

    private void ProcessManualDiscard() {
        while (true) {
            var state = Game.State;
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
                .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
                .Where(n => n >= 0 && n < state.CurrentHand.Count)
                .ToList();

            if (indices == null || indices.Count == 0 || indices.Count > Game.Config.MaxDiscardCards) {
                Console.WriteLine($"Error. Please select between 1 and {Game.Config.MaxDiscardCards} valid cards!");
                continue;
            }

            var cardsToDiscard = indices.Select(i => state.CurrentHand[i]).ToList();
            ProcessDiscard(cardsToDiscard);
            break;
        }
    }

    private void ProcessDiscard(List<Card> cardsToDiscard) {
        Console.WriteLine($"Discarded: {string.Join(", ", cardsToDiscard)}");
        var result = Game.Discard(cardsToDiscard);
        Game.DrawCards();
        Console.WriteLine($"* Cards discarded. Remaining discards: {Game.Config.MaxDiscards - Game.State.Discards}");
        Console.WriteLine($"* New hand: {string.Join(", ", Game.State.CurrentHand)}");
        if (!autoPlay) {
            Console.WriteLine("(press any key to continue)");
            Console.ReadKey();
        }
    }

    private const float Risk = 0.7f;
}