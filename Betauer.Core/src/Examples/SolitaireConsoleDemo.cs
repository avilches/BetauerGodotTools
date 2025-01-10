using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Examples;

public class SolitaireConsoleDemo(Random random) {
    public static void Main() {
        new SolitaireConsoleDemo(new Random(1)).Play();
    }

    const float Risk = 0.7f;
    public SolitairePokerGame Game;

    public void Play() {
        while (true) {
            Console.WriteLine("Welcome to Solitaire Poker!");

            Game = new SolitairePokerGame(random, new PokerGameConfig());
            Game.Hands.RegisterBasicPokerHands();

            Game.DrawCards();
            while (!Game.IsGameOver()) {
                DisplayGameState();
                if (ProcessUserInput() == true) {
                    break;
                }
            }
        }
    }

    private void DisplayGameState() {
        var state = Game.State;
        Console.Clear();
        Console.WriteLine($"=== Solitaire Poker - Hand {state.HandsPlayed + 1}/{Game.Config.MaxHands} ===");
        Console.WriteLine($"Total Score: {state.TotalScore}");
        Console.WriteLine($"Discards: {state.Discards}/{Game.Config.MaxDiscards}");
        Console.WriteLine("\nYour hand:");
        DisplayCards(state.CurrentHand);
        Console.WriteLine("\nHistory:");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == GameHistory.GameActionType.Play)
                Console.WriteLine($"Played: {action.PlayedHand} (Score: {action.Score})");
            else
                Console.WriteLine($"Discarded: {string.Join(" ", action.Cards)}");
        }
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
                Console.WriteLine($"   - {handType.Name,-20} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]":"[ ]")} {(handType == currentBestHand.GetType()?"redundant": "")}");
                // Console.WriteLine($"   - {handType.Name}");
                // Console.WriteLine($"     Score range: {stats.MinScore}-{stats.MaxScore} (avg: {stats.AvgScore:F2})");
                // Console.WriteLine($"     Probability: {probability:P2} ({stats.Count}/{option.TotalSimulations})");
                // Console.WriteLine($"     Potential Score: {potentialScore:F2}");
            }
            i++;
        }
        foreach (var option in options.Where(option => option.GetBestPotentialScore(Risk) <= Risk)) {
            var discardBestHand = option.GetBestHand(0)!;
            Console.WriteLine($"\n{i}. Discarding {option.CardsToDiscard.Count}: {string.Join(" ", option.CardsToDiscard)}, keeping: {string.Join(" ", option.CardsToKeep)}  Score: {discardBestHand.PotentialScore:F2}");
            var handsByScore = option.HandOccurrences
                .OrderByDescending(kv => kv.Value.PotentialScore);

            foreach (var (handType, stats) in handsByScore) {
                Console.WriteLine($"   - {handType.Name,-20} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]":"[ ]")} {(handType == currentBestHand.GetType()?"redundant": "")}");
                // Console.WriteLine($"   - {handType.Name}");
                // Console.WriteLine($"     Score range: {stats.MinScore}-{stats.MaxScore} (avg: {stats.AvgScore:F2})");
                // Console.WriteLine($"     Probability: {probability:P2} ({stats.Count}/{option.TotalSimulations})");
                // Console.WriteLine($"     Potential Score: {potentialScore:F2}");
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
        Console.WriteLine("A: Auto play: "+GetAutoPlay(possibleHands, discardOptions));
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
                ProcessAutoHand(hand);
            } else {
                choice -= possibleHands.Count;
                var cardsToDiscard = discardOptions.DiscardOptions[choice - 1].CardsToDiscard;
                ProcessAutoDiscard(cardsToDiscard);
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
            return "No more discards. Play best hand "+possibleHands[0];
        }
        
        // HACER LOS TESTS DE LOS DESCARTES
            // - EL DESCARTE NO DEBERIA VALER LA SUMA DE LAS MANOS POTENCIALES, SINO LA PUNTUACION MAS ALTA
            // QUE YA VIENE "REBAJADA" POR LA PROBABILIDAD
                // - LA PUNTUACION DEBE TENER FICHAS X MULTI

        var bestHand = possibleHands[0];
        var bestDiscardOption = discardOptions.GetBestDiscards(Risk).FirstOrDefault();

        var bestDiscardHandScore = bestDiscardOption?.GetBestPotentialScore(Risk) ?? 0;
        if (bestHand.CalculateScore() >= bestDiscardHandScore) {
            return $"Best hand ({possibleHands[0]} scores {bestHand.CalculateScore():0} >= potential discard ({string.Join(", ", bestDiscardOption?.CardsToDiscard??[])}) score {bestDiscardHandScore:0})";
        } else {
            return $"Best discard ({possibleHands[0]} scores {bestHand.CalculateScore():0} < potential discard ({string.Join(", ", bestDiscardOption?.CardsToDiscard??[])}) score {bestDiscardHandScore:0})";
        }
    }

    private void ProcessAutoPlay(List<PokerHand> possibleHands, DiscardOptionsResult discardOptions) {
        // Si no podemos descartarnos más, jugamos la mejor mano disponible
        if (!Game.CanDiscard()) {
            ProcessAutoHand(possibleHands[0]);
        }

        var bestHand = possibleHands[0];
        var bestDiscardOption = discardOptions.GetBestDiscards(Risk).FirstOrDefault();

        var bestDiscardHandScore = bestDiscardOption?.GetBestPotentialScore(Risk) ?? 0;
        if (bestHand.CalculateScore() >= bestDiscardHandScore) {
            ProcessAutoHand(bestHand);
        } else {
            ProcessAutoDiscard(bestDiscardOption!.CardsToDiscard);
        }
    }

    private void ProcessAutoDiscard(List<Card> cardsToDiscard) {
        try {
            Console.WriteLine("Auto discarding: " + string.Join(", ", cardsToDiscard));
            var result = Game.Discard(cardsToDiscard);
            Game.DrawCards();
            Console.WriteLine("* New hand: " + string.Join(", ", Game.State.CurrentHand));
        } catch (SolitairePokerGameException e) {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("(press any key to continue)");
        Console.ReadKey();
    }

    private void ProcessAutoHand(PokerHand hand) {
        try {
            var result = Game.PlayHand(hand);
            Console.WriteLine($"Played {hand.GetType().Name}: {string.Join(", ", hand)}. Scored: +{result.Score}");
            Game.DrawCards();
            Console.WriteLine("* New hand: " + string.Join(", ", Game.State.CurrentHand));
        } catch (SolitairePokerGameException e) {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("(press any key to continue)");
        Console.ReadKey();
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

            try {
                var hand = possibleHands[0];
                var result = Game.PlayHand(hand);
                Console.WriteLine($"Played {hand.GetType().Name}: {string.Join(", ", hand)}. Scored: +{result.Score}");
                Console.WriteLine("* New hand: " + string.Join(", ", Game.State.CurrentHand));
                Console.WriteLine("(press any key to continue)");
                Console.ReadKey();
                break;
            } catch (SolitairePokerGameException e) {
                Console.WriteLine(e.Message);
            }
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
            try {
                DisplayCards(Game.State.CurrentHand);
                var result = Game.Discard(cardsToDiscard);
                Console.WriteLine("Discarded cards: " + string.Join(", ", result.DiscardedCards));
                Game.DrawCards();
                Console.WriteLine("New hand: " + string.Join(", ", Game.State.CurrentHand));
                Console.WriteLine("(press any key to continue)");
                Console.ReadKey();
                break;
            } catch (SolitairePokerGameException e) {
                Console.WriteLine(e.Message);
            }
        }
    }

    private void DisplayEndGame() {
        var state = Game.State;
        Console.Clear();
        Console.WriteLine("=== Game Over ===");
        Console.WriteLine($"Final Score: {state.TotalScore}");
        Console.WriteLine("\nHands played:");
        foreach (var action in state.History.GetHistory()) {
            if (action.Type == GameHistory.GameActionType.Play)
                Console.WriteLine($"Played: {action.PlayedHand} (Score: {action.Score})");
            else
                Console.WriteLine($"Discarded: {string.Join(" ", action.Cards)}");
        }
    }
}