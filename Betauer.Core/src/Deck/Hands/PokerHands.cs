using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck.Hands;

public record PokerHandConfig(PokerHand Prototype, int Multiplier);

public class PokerHands {
    private readonly List<PokerHandConfig> _handConfigs = [];

    public void RegisterBasicPokerHands() {
        RegisterHand(new HighCardHand(this, []), 1);
        RegisterHand(new PairHand(this, []), 2);
        RegisterHand(new TwoPairHand(this, []), 3);
        RegisterHand(new ThreeOfAKindHand(this, []), 4);
        RegisterHand(new StraightHand(this, []), 5);
        RegisterHand(new FlushHand(this, []), 6);
        RegisterHand(new FullHouseHand(this, []), 7);
        RegisterHand(new FourOfAKindHand(this, []), 8);
        RegisterHand(new StraightFlushHand(this, []), 9);
    }

    /// <summary>
    /// Registers a poker hand with its corresponding multiplier.
    /// If a hand of the same type already exists, it will be replaced.
    /// Hand configs are kept sorted by multiplier in descending order.
    /// </summary>
    public void RegisterHand(PokerHand prototype, int multiplier) {
        // Remove any existing config for this hand type if it exists
        _handConfigs.RemoveAll(config => config.Prototype.GetType() == prototype.GetType());
        _handConfigs.Add(new PokerHandConfig(prototype, multiplier));
        // Re-sort configs by multiplier in descending order
        _handConfigs.Sort((a, b) => b.Multiplier.CompareTo(a.Multiplier));
    }

    /// <summary>
    /// Identifies all possible poker hands in the given cards.
    /// Returns hands ordered by score in descending order.
    /// For multiple hands of the same type, they are numbered (#1, #2, etc.).
    /// </summary>
    /// <param name="cards">Cards to analyze</param>
    /// <returns>List of identified poker hands, ordered by score</returns>
    public List<PokerHand> IdentifyAllHands(IReadOnlyList<Card> cards) {
        if (_handConfigs.Count == 0) {
            throw new InvalidOperationException("No hands registered");
        }
        if (cards.Count == 0) return [];

        var allHands = _handConfigs
            .SelectMany(config => config.Prototype.IdentifyHands(cards))
            .OrderByDescending(CalculateScore)
            .ToList();

        // Add unique identifier for hands of the same type
        var groupedHands = allHands.GroupBy(h => h.Name).ToList();
        foreach (var group in groupedHands.Where(g => g.Count() > 1)) {
            var i = 1;
            foreach (var hand in group) {
                hand.Name = $"{hand.Name} #{i}";
                i++;
            }
        }

        return allHands;
    }

    /// <summary>
    /// Analyzes current hand and provides discard options for improvement.
    /// Uses Monte Carlo simulation to estimate probabilities when exact calculation
    /// is impractical.
    /// 
    /// Algorithm:
    /// 1. For each hand type, if the hand doesn't already exist:
    ///    - Get suggested discards for that hand type
    /// 2. For each unique discard combination:
    ///    - Simulate drawing new cards multiple times
    ///    - Identify best possible hand for each simulation
    ///    - Track statistics for each hand type achieved
    /// 3. Calculate probabilities and potential scores
    /// 4. Return options ordered by potential score
    /// </summary>
    /// <param name="currentHand">Current cards in hand</param>
    /// <param name="neverDiscard">Cards in hand that can't be discarded</param>
    /// <param name="availableCards">Cards available to draw</param>
    /// <param name="maxDiscardCards">Maximum number of cards that can be discarded</param>
    /// <returns>Analysis results with discard options and statistics</returns>
    public DiscardOptionsResult GetDiscardOptions(IReadOnlyList<Card> currentHand, IReadOnlyList<Card> neverDiscard, IReadOnlyList<Card> availableCards, int maxDiscardCards) {
        if (maxDiscardCards < 0) throw new ArgumentException("maxDiscardCards cannot be negative");

        const int MaxSimulations = 10000;
        const double MinSimulationPercentage = 0.10;
        
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var totalSimulations = 0;
        var totalCombinations = 0;

        // Get all possible discard combinations from all hand types
        var suggestedDiscards = _handConfigs
            .SelectMany(config => config.Prototype.GetBestDiscards(currentHand, maxDiscardCards))
            .Where(cardsToDiscard => cardsToDiscard != null && cardsToDiscard.Count > 0)
            .Where(cardsToDiscard => !cardsToDiscard.Any(neverDiscard.Contains))
            .Distinct(new CardListEqualityComparer())
            .ToList();


        var options = new List<DiscardOption>();
        var random = new Random();

        // Analyze each discard combination
        foreach (var cardsToDiscard in suggestedDiscards) {
            var cardsToKeep = currentHand.Except(cardsToDiscard).ToList();
            var handTypeOccurrences = new Dictionary<Type, HandTypeStats>();
            
            // Calculate total possible combinations
            int combinations = CombinationTools.Calculate(availableCards.Count, cardsToDiscard.Count);
            if (combinations == 0) continue;
            totalCombinations += combinations;

            // Determine number of simulations
            var simulations = Math.Min(
                MaxSimulations,
                Math.Max(
                    (int)(combinations * MinSimulationPercentage),
                    combinations
                )
            );
            totalSimulations += simulations;

            var availableCardsList = availableCards.ToList();
            // Generate and analyze random combinations
            var draws = simulations == combinations
                ? availableCardsList.Combinations(cardsToDiscard.Count)
                    .Select(combo => combo.ToList())
                : availableCardsList.RandomCombinations(cardsToDiscard.Count, simulations, random);

            foreach (var draw in draws) {
                var newHand = new List<Card>(cardsToKeep);
                newHand.AddRange(draw);

                var bestHand = IdentifyAllHands(newHand).MaxBy(h => h.CalculateScore());
                if (bestHand != null) {
                    var handType = bestHand.GetType();
                    var score = bestHand.CalculateScore();
                    if (!handTypeOccurrences.TryGetValue(handType, out HandTypeStats? value)) {
                        handTypeOccurrences[handType] = new HandTypeStats(score);
                    } else {
                        value.AddScore(score);
                    }
                }
            }

            if (handTypeOccurrences.Any()) {
                options.Add(new DiscardOption(
                    cardsToKeep,
                    cardsToDiscard,
                    handTypeOccurrences,
                    simulations,
                    combinations));
            }
        }

        watch.Stop();
        return new DiscardOptionsResult(
            options,
            watch.Elapsed,
            totalSimulations,
            totalCombinations
        );
    }
    
    private class CardListEqualityComparer : IEqualityComparer<List<Card>> {
        public bool Equals(List<Card>? x, List<Card>? y) {
            if (x == null || y == null) return x == y;
            return x.Count == y.Count && x.All(y.Contains);
        }

        public int GetHashCode(List<Card> obj) {
            return obj.Aggregate(0, (current, card) => current ^ card.GetHashCode());
        }
    }

    public PokerHandConfig GetPokerHandConfig(PokerHand hand) {
        if (_handConfigs.Count == 0) {
            throw new InvalidOperationException("No hands registered");
        }
        var config = _handConfigs.Find(c => c.Prototype.GetType() == hand.GetType());
        if (config == null) {
            throw new ArgumentException($"No configuration found for hand type: {hand.GetType()}");
        }
        return config;
    }

    public int CalculateScore(PokerHand hand) {
        return hand.Cards.Sum(c => c.Rank) * GetPokerHandConfig(hand).Multiplier;
    }
}