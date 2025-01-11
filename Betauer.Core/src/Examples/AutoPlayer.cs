using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;

namespace Betauer.Core.Examples;

public class AutoPlayer {
    public record AutoPlayDecision(
        bool ShouldPlay,
        string Reason,
        PokerHand? HandToPlay,
        DiscardOption? DiscardOption,
        List<PokerHand> PossibleHands,
        DiscardOptionsResult DiscardOptions
    );

    public AutoPlayDecision GetNextAction(SolitairePokerGame game) {
        var currentHands = game.GetPossibleHands();
        var bestCurrentHand = currentHands[0];
        var currentScore = bestCurrentHand.CalculateScore();
        var discardOptions = game.GetDiscardOptions(bestCurrentHand.Cards);

        var loosing = game.RemainingHands == 1 && currentScore < game.RemainingScoreToWin;

        // Con esta mano ganamos? jugar!
        if (currentScore >= game.RemainingScoreToWin) {
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
        if (!game.CanDiscard()) {
            return new AutoPlayDecision(
                true,
                (loosing ? ":-o Losing": ":(")+" No discards remaining, must play current hand",
                HandToPlay: bestCurrentHand,
                DiscardOption: null,
                PossibleHands: currentHands,
                DiscardOptions: discardOptions
            );
        }

        // Si es la última mano y no ganamos, pero tenemos descartes, descartar para ver si tenemos mas suerte la proxima
        if (game.RemainingHands == 1 && currentScore < game.RemainingScoreToWin) {
            var firstDiscard = discardOptions.DiscardOptions.FirstOrDefault();
            if (firstDiscard != null) {
                return new AutoPlayDecision(
                    false,
                    $":-/ Current hand score {currentScore} too low to win the last hand. We need {game.RemainingScoreToWin} to win",
                    HandToPlay: null,
                    DiscardOption: firstDiscard,
                    PossibleHands: currentHands,
                    DiscardOptions: discardOptions
                );
            } else {
                Console.WriteLine("!!");
            }
        }

        // No ganamos si jugamos, no es la última mano y quedan descartes
        
        var minimumScoreNeeded = CalculateMinimumScoreNeeded(game);
        
        // Si la mano actual es muy mala, descartamos sin importar el riesgo
        if (currentScore < minimumScoreNeeded / 2) {
            var firstDiscard = discardOptions.DiscardOptions.FirstOrDefault();
            if (firstDiscard != null) {
                return new AutoPlayDecision(
                    false,
                    $":-( Current hand score {currentScore} too low. Every hand should score: {minimumScoreNeeded}",
                    HandToPlay: null,
                    DiscardOption: firstDiscard,
                    PossibleHands: currentHands,
                    DiscardOptions: discardOptions
                );
            } else {
                Console.WriteLine("!!");
            }
        } else if (currentScore >= minimumScoreNeeded) {
            // Si la mano actual es suficientemente buena como para ganar la , la jugamos
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
        var risk = CalculateDynamicRisk(game);
        var bestDiscard = GetBestValidDiscardOption(risk, currentHands, discardOptions, game);

        if (bestDiscard != null) {
            var bestHand = bestDiscard.GetBestHand(risk);
            if (bestHand != null && bestHand.PotentialScore >= currentScore) {
                return new AutoPlayDecision(
                    false,
                    $"Better discard score {bestHand.PotentialScore} >= hand {currentScore}. Risk: {risk:0.00}",
                    HandToPlay: null,
                    DiscardOption: bestDiscard,
                    PossibleHands: currentHands,
                    DiscardOptions: discardOptions
                );
            } else {
                // Si no hay mejor opción, jugamos la mano actual
                return new AutoPlayDecision(
                    true,
                    $"No better discard available. Discard score {bestHand?.PotentialScore ?? 0} < hand {currentScore}. Risk: {risk:0.00}",
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

    private float CalculateDynamicRisk(SolitairePokerGame game) {
        return CalculateDynamicRisk(game.RemainingHands, game.RemainingDiscards);
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
            Console.WriteLine($"CalculateDynamicRisk: Fixed risk 0.4 because {remainingDiscards} >= {remainingHands}");
            return 0.4f; // Risk base cuando hay suficientes descartes
        }

        // El risk sube cuando hay menos descartes que manos
        var risk = 0.4f + (0.5f * (1f - (float)remainingDiscards / remainingHands));
        Console.WriteLine($"CalculateDynamicRisk: RemainingDiscards:{remainingDiscards}, RemainingHands:{remainingHands}, 1-RemainingDiscards / RemainingHands:{1-((float)remainingDiscards / remainingHands)}. Risk: {risk}");
        return risk;
    }

    public static float CalculateMinimumScoreNeeded(SolitairePokerGame game) {
        if (game.RemainingHands == 0) return 0;
        var remainingScore = game.State.TotalScore - game.State.Score;
        return (float)remainingScore / game.RemainingHands;
    }

    private DiscardOption? GetBestValidDiscardOption(float risk, List<PokerHand> currentHands, DiscardOptionsResult discardOptions, SolitairePokerGame game) {
        var currentBestHandType = currentHands[0].GetType();

        return discardOptions.GetBestDiscards(risk)
            .FirstOrDefault(option => {
                var bestHand = option.GetBestHand(risk);
                return bestHand != null && bestHand.GetType() != currentBestHandType;
            });
    }
}