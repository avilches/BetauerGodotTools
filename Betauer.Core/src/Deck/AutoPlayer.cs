using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck.Hands;
using Godot;

namespace Betauer.Core.Deck;

public class AutoPlayer {
    public record AutoPlayDecision(
        bool ShouldPlay,
        string Reason,
        PokerHand? HandToPlay,
        DiscardOption? DiscardOption,
        List<PokerHand> PossibleHands,
        DiscardOptionsResult DiscardOptions
    );

    public AutoPlayDecision GetNextAction(GameHandler handler) {
        var currentHands = handler.GetPossibleHands();

        // Keep only the 
        var bestHighCard = currentHands.FirstOrDefault(p => p.GetType() == typeof(HighCardHand));
        currentHands.RemoveAll(p => p.GetType() == typeof(HighCardHand) && p != bestHighCard);
        
        
        var bestCurrentHand = currentHands[0];
        var currentScore = handler.CalculateScore(bestCurrentHand);
        var discardOptions = handler.GetDiscardOptions();

        var loosing = handler.RemainingHands == 1 && currentScore < handler.RemainingScoreToWin;

        // Con esta mano ganamos? jugar!
        if (currentScore >= handler.RemainingScoreToWin) {
            return new AutoPlayDecision(
                true,
                ":-) Play to win!",
                HandToPlay: bestCurrentHand,
                DiscardOption: null,
                PossibleHands: currentHands,
                DiscardOptions: discardOptions
            );
        }

        // Si no quedan descartes, no queda mas remedio que jugar
        if (!handler.CanDiscard()) {
            return new AutoPlayDecision(
                true,
                (loosing ? ":-o Losing": ":(")+" No discards remaining, must play current hand",
                HandToPlay: bestCurrentHand,
                DiscardOption: null,
                PossibleHands: currentHands,
                DiscardOptions: discardOptions
            );
        }

        // Remove a discard that the best hand is the same hand
        
        
        
        var bestDiscard = discardOptions.Discards.FirstOrDefault(o => o.GetBestHand().HandType != bestCurrentHand.GetType());

        // Si es la última mano y no ganamos, pero tenemos descartes, descartar para ver si tenemos mas suerte la proxima
        if (handler.RemainingHands == 1 && currentScore < handler.RemainingScoreToWin) {
            if (bestDiscard != null) {
                return new AutoPlayDecision(
                    false,
                    $":-/ Current hand score {currentScore} too low to win the last hand. We need {handler.RemainingScoreToWin} to win",
                    HandToPlay: null,
                    DiscardOption: bestDiscard,
                    PossibleHands: currentHands,
                    DiscardOptions: discardOptions
                );
            }
        }

        // No ganamos si jugamos, no es la última mano y quedan descartes
        
        var minimumScoreNeeded = CalculateMinimumScoreNeeded(handler);
        
        // Si la mano actual es muy mala, descartamos sin importar el riesgo
        if (currentScore < minimumScoreNeeded / 2) {
            if (bestDiscard != null) {
                return new AutoPlayDecision(
                    false,
                    $":-( Current hand score {currentScore} is too low. Every hand should score: {minimumScoreNeeded}",
                    HandToPlay: null,
                    DiscardOption: bestDiscard,
                    PossibleHands: currentHands,
                    DiscardOptions: discardOptions
                );
            }
        } else if (currentScore >= minimumScoreNeeded) {
            // Si la mano actual es suficientemente buena como para ganar la parte que toca del juego actual, la jugamos
            return new AutoPlayDecision(
                true,
                $":-) Current hand score {currentScore} meets requirements. Every hand should score: {minimumScoreNeeded}",
                HandToPlay: bestCurrentHand,
                DiscardOption: null,
                PossibleHands: currentHands,
                DiscardOptions: discardOptions
            );
        }

        // La mano actual vale entre [minimumScoreNeeded/2 y minimumScoreNeeded]
        var risk = CalculateDynamicRisk(handler);

        if (bestDiscard != null) {
            var bestHand = bestDiscard.GetBestHand();
            if (bestHand.PotentialScore >= currentScore * 1.3f) {
                return new AutoPlayDecision(
                    false,
                    $"Better discard score {bestHand.PotentialScore} 30% bigger than your current hand {currentScore}. Risk: {risk:0.00}",
                    HandToPlay: null,
                    DiscardOption: bestDiscard,
                    PossibleHands: currentHands,
                    DiscardOptions: discardOptions
                );
            } else {
                // Si no hay mejor opción, jugamos la mano actual
                return new AutoPlayDecision(
                    true,
                    $"No better discard available. Best discard score {bestHand?.PotentialScore ?? 0} is not 30% bigger than your current hand {currentScore}. Risk: {risk:0.00}",
                    HandToPlay: bestCurrentHand,
                    DiscardOption: null,
                    PossibleHands: currentHands,
                    DiscardOptions: discardOptions
                );
            }
        } else {
            // Si no hay mejor opción, jugamos la mano actual
            return new AutoPlayDecision(
                true,
                $"No discard available. Risk: {risk:0.00}",
                HandToPlay: bestCurrentHand,
                DiscardOption: null,
                PossibleHands: currentHands,
                DiscardOptions: discardOptions
            );

        }

    }

    private float CalculateDynamicRisk(GameHandler gameHandler) {
        return CalculateDynamicRisk(gameHandler.RemainingHands, gameHandler.RemainingDiscards);
    }

    public static void Main() {
        Console.WriteLine("RemainingHands:4");
        CalculateDynamicRisk(4, 4);
        CalculateDynamicRisk(4, 3);
        CalculateDynamicRisk(4, 2);
        CalculateDynamicRisk(4, 1);
        CalculateDynamicRisk(4, 0);
        Console.WriteLine("RemainingHands:3");
        CalculateDynamicRisk(3, 4);
        CalculateDynamicRisk(3, 3);
        CalculateDynamicRisk(3, 2);
        CalculateDynamicRisk(3, 1);
        CalculateDynamicRisk(3, 0);
        Console.WriteLine("RemainingHands:2");
        CalculateDynamicRisk(2, 4);
        CalculateDynamicRisk(2, 3);
        CalculateDynamicRisk(2, 2);
        CalculateDynamicRisk(2, 1);
        CalculateDynamicRisk(2, 0);
        Console.WriteLine("RemainingHands:1");
        CalculateDynamicRisk(1, 4);
        CalculateDynamicRisk(1, 3);
        CalculateDynamicRisk(1, 2);
        CalculateDynamicRisk(1, 1);
        CalculateDynamicRisk(1, 0);
        Console.WriteLine("RemainingHands:0");
        CalculateDynamicRisk(0, 4);
        CalculateDynamicRisk(0, 3);
        CalculateDynamicRisk(0, 2);
        CalculateDynamicRisk(0, 1);
        CalculateDynamicRisk(0, 0);
    }

    public static float CalculateDynamicRisk(int remainingHands, int remainingDiscards) {
        if (remainingDiscards >= remainingHands) {
            // Console.WriteLine($"CalculateDynamicRisk: Fixed risk 0.4 because {remainingDiscards} >= {remainingHands}");
            return 0.4f; // Risk base cuando hay suficientes descartes
        }

        // El risk sube cuando hay menos descartes que manos
        var risk = 0.4f + (0.5f * (1f - (float)remainingDiscards / remainingHands));
        // Console.WriteLine($"CalculateDynamicRisk: RemainingDiscards:{remainingDiscards}, RemainingHands:{remainingHands}, 1-RemainingDiscards / RemainingHands:{1-((float)remainingDiscards / remainingHands)}. Risk: {risk}");
        return risk;
    }

    public static int CalculateMinimumScoreNeeded(GameHandler gameHandler) {
        if (gameHandler.RemainingHands == 0) return 0;
        var remainingScore = gameHandler.State.TotalScore - gameHandler.State.Score;
        return Mathf.RoundToInt((float)remainingScore / gameHandler.RemainingHands);
    }
}