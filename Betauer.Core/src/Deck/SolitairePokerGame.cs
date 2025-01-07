using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public class SolitairePokerGame {
    private readonly Deck deck;
    private readonly HandIdentifier handIdentifier;
    private readonly GameHistory history;
    private List<Card> currentHand;
    private int remainingDiscards;
    private int totalScore;
    private int handsPlayed;

    private const int MAX_DISCARDS = 4;
    private const int MAX_DISCARD_CARDS = 5;
    private const int HAND_SIZE = 7;
    private const int MAX_HANDS = 4;

    public SolitairePokerGame() {
        deck = new Deck();
        handIdentifier = new HandIdentifier();
        history = new GameHistory();
        remainingDiscards = MAX_DISCARDS;
        totalScore = 0;
        handsPlayed = 0;
    }

     private void DisplayGameState() {
        Console.Clear();
        Console.WriteLine($"=== Solitaire Poker - Hand {handsPlayed + 1}/4 ===");
        Console.WriteLine($"Total Score: {totalScore}");
        Console.WriteLine($"Remaining Discards: {remainingDiscards}");
        Console.WriteLine("\nYour hand:");
        DisplayCards(currentHand);
        Console.WriteLine("\nHistory:");
        foreach (var action in history.GetHistory()) {
            if (action.Type == "PLAY")
                Console.WriteLine($"Played: {action.PlayedHand} (Score: {action.Score})");
            else
                Console.WriteLine($"Discarded: {string.Join(" ", action.Cards)}");
        }
    }

    private void DisplayCards(List<Card> cards) {
        // Agrupar por palo y ordenar por rank descendente
        var groupedCards = cards
            .GroupBy(c => c.Suit)
            .OrderBy(g => g.Key)
            .ToList();

        foreach (var group in groupedCards) {
            var sortedCards = group.OrderByDescending(c => c.Rank);
            Console.WriteLine($"{group.Key}: {string.Join(" ", sortedCards)}");
        }
    }

    private void DisplayPossibleHands(List<PokerHand> hands) {
        Console.WriteLine("\nPossible hands you can play:");
        for (int i = 0; i < hands.Count; i++) {
            Console.WriteLine($"{i + 1}: {hands[i]}");
        }
    }

public void Play() {
        Console.WriteLine("Welcome to Solitaire Poker!");
        deck.Shuffle();

        while (handsPlayed < MAX_HANDS) {
            if (currentHand == null) {
                currentHand = deck.Draw(HAND_SIZE);
            }

            DisplayGameState();
            var possibleHands = handIdentifier.IdentifyAllHands(currentHand);
            DisplayPossibleHands(possibleHands);

            Console.WriteLine("\nOptions:");
            Console.WriteLine("1-N: Play a hand from the list");
            Console.WriteLine("M: Make your own hand by selecting cards");
            Console.WriteLine("D: Discard cards");
            Console.WriteLine("Q: Quit game");

            var option = Console.ReadLine()?.ToUpper();
            
            if (option == "Q") {
                EndGame();
                return;
            }
            
            if (option == "M") {
                PlayManualHand();
            }
            else if (option == "D") {
                if (remainingDiscards > 0)
                    PerformDiscard();
                else
                    Console.WriteLine("No discards remaining!");
            }
            else if (int.TryParse(option, out int choice) && choice > 0 && choice <= possibleHands.Count) {
                var selectedHand = possibleHands[choice - 1];
                PlaySelectedHand(selectedHand);
            }
            else {
                Console.WriteLine("Invalid option!");
            }
        }

        EndGame();
    }

    private void PlaySelectedHand(PokerHand selectedHand) {
        // Registrar la jugada y actualizar puntuación
        totalScore += selectedHand.Score;
        history.AddPlay(selectedHand);
        handsPlayed++;

        // Mantener las cartas no usadas
        var remainingCards = currentHand.Where(c => !selectedHand.Cards.Contains(c)).ToList();
        
        // Si quedan más manos por jugar, robar cartas nuevas solo para reemplazar las usadas
        if (handsPlayed < MAX_HANDS) {
            var newCards = deck.Draw(HAND_SIZE - remainingCards.Count);
            currentHand = remainingCards.Concat(newCards).ToList();
        } else {
            currentHand = null;
        }
    }

    private void PlayManualHand() {
        Console.WriteLine("\nSelect cards to play (enter card positions, e.g., '1 3 5'):");
        Console.WriteLine("Current hand:");
        for (int i = 0; i < currentHand.Count; i++) {
            Console.WriteLine($"{i + 1}: {currentHand[i]}");
        }

        var input = Console.ReadLine();
        var selectedIndices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
            .Where(n => n >= 0 && n < currentHand.Count)
            .ToList();

        if (selectedIndices == null || selectedIndices.Count == 0) {
            Console.WriteLine("Invalid selection!");
            return;
        }

        var selectedCards = selectedIndices.Select(i => currentHand[i]).ToList();
        var possibleHands = handIdentifier.IdentifyAllHands(selectedCards);

        if (possibleHands.Count == 0) {
            Console.WriteLine("These cards don't form a valid poker hand!");
            return;
        }

        // Usar la mejor mano posible con las cartas seleccionadas
        PlaySelectedHand(possibleHands[0]);
    }
    private void PerformDiscard() {
        Console.WriteLine("\nEnter the indices of cards to discard (1-7, separated by spaces):");
        Console.WriteLine("Current hand:");
        for (int i = 0; i < currentHand.Count; i++) {
            Console.WriteLine($"{i + 1}: {currentHand[i]}");
        }

        var input = Console.ReadLine();
        var indices = input?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => int.TryParse(s, out int n) ? n - 1 : -1)
            .Where(n => n >= 0 && n < currentHand.Count)
            .ToList();

        if (indices == null || indices.Count == 0 || indices.Count > MAX_DISCARD_CARDS) {
            Console.WriteLine($"Please select between 1 and {MAX_DISCARD_CARDS} valid cards!");
            return;
        }

        var cardsToDiscard = indices.Select(i => currentHand[i]).ToList();
        var remainingCards = currentHand.Except(cardsToDiscard).ToList();

        deck.ReturnCards(cardsToDiscard);
        deck.Shuffle();
        var newCards = deck.Draw(cardsToDiscard.Count);
        currentHand = remainingCards.Concat(newCards).ToList();

        history.AddDiscard(cardsToDiscard);
        remainingDiscards--;
    }

    private void EndGame() {
        Console.Clear();
        Console.WriteLine("=== Game Over ===");
        Console.WriteLine($"Final Score: {totalScore}");
        Console.WriteLine("\nHands played:");
        foreach (var action in history.GetHistory()) {
            if (action.Type == "PLAY")
                Console.WriteLine($"Played: {action.PlayedHand} (Score: {action.Score})");
            else
                Console.WriteLine($"Discarded: {string.Join(" ", action.Cards)}");
        }
    }
}

// Program.cs
public class Program {
    public static void Main() {
        var game = new SolitairePokerGame();
        game.Play();
    }
}