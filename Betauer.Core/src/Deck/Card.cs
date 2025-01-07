using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.Deck;

public readonly struct Card(int rank, char suit) : IComparable<Card> {
    public int Rank { get; } = rank;
    public char Suit { get; } = suit;

    public override string ToString() {
        var rankStr = Rank switch {
            14 => "A",
            13 => "K",
            12 => "Q",
            11 => "J",
            10 => "T",
            _ => Rank.ToString()
        };
        return $"{rankStr}{Suit}";
    }

    public static Card Parse(string str) {
        if (string.IsNullOrEmpty(str) || str.Length != 2)
            throw new ArgumentException("Card string must be 2 characters");

        int rank = str[0] switch {
            'A' => 14,
            'K' => 13,
            'Q' => 12,
            'J' => 11,
            'T' => 10,
            _ when char.IsDigit(str[0]) => int.Parse(str[0].ToString()),
            _ => throw new ArgumentException("Invalid rank")
        };

        if (!("SHDC").Contains(str[1]))
            throw new ArgumentException("Invalid suit");

        return new Card(rank, str[1]);
    }

    public int CompareTo(Card other) => Rank.CompareTo(other.Rank);
}

public class Deck {
    private readonly List<Card> _cards = new();
    private readonly Random random = new();
    private readonly PokerGameConfig config;

    public Deck() {
        foreach (char suit in "SHDC") {
            for (int rank = 2; rank <= 14; rank++) {
                _cards.Add(new Card(rank, suit));
            }
        }
    }

    public void Shuffle() {
        int n = _cards.Count;
        while (n > 1) {
            n--;
            int k = random.Next(n + 1);
            (_cards[k], _cards[n]) = (_cards[n], _cards[k]);
        }
    }

    public IReadOnlyList<Card> Draw(int count) {
        if (count > _cards.Count) throw new InvalidOperationException("Not enough cards in deck");
        var drawn = _cards.Take(count).ToList();
        _cards.RemoveRange(0, count);
        return drawn;
    }

    public void ReturnCards(IReadOnlyList<Card> returnedCards) {
        _cards.AddRange(returnedCards);
    }

    public int RemainingCards => _cards.Count;
    
    public Deck Clone() {
        var newDeck = new Deck();
        newDeck._cards.Clear();
        newDeck._cards.AddRange(_cards);
        return newDeck;
    }
}

public class PokerGameConfig {
    public int HandSize { get; set; } = 7;
    public int MaxHands { get; set; } = 4;
    public int MaxDiscards { get; set; } = 3;
    public int MaxDiscardCards { get; set; } = 3;
    public int MinRank { get; set; } = 2;
    public int MaxRank { get; set; } = 14;
    public string ValidSuits { get; set; } = "SHDC";
    
    public PokerGameConfig Clone() {
        return new PokerGameConfig {
            HandSize = HandSize,
            MaxHands = MaxHands,
            MaxDiscards = MaxDiscards,
            MaxDiscardCards = MaxDiscardCards,
            MinRank = MinRank,
            MaxRank = MaxRank,
            ValidSuits = ValidSuits
        };
    }
}

public class GameHistory {
    private readonly List<GameAction> actions = new();

    public class GameAction {
        public string Type { get; }
        public IReadOnlyList<Card> Cards { get; }
        public PokerHand? PlayedHand { get; }
        public int Score { get; }

        public GameAction(string type, IReadOnlyList<Card> cards, PokerHand? playedHand = null, int score = 0) {
            Type = type;
            Cards = cards;
            PlayedHand = playedHand;
            Score = score;
        }
    }

    public void AddPlay(PokerHand hand, int score) {
        actions.Add(new GameAction("PLAY", hand.Cards, hand, score));
    }

    public void AddDiscard(IReadOnlyList<Card> cards) {
        actions.Add(new GameAction("DISCARD", cards));
    }

    public IReadOnlyList<GameAction> GetHistory() => actions.AsReadOnly();

    public GameHistory Clone() {
        var newHistory = new GameHistory();
        foreach (var action in actions) {
            if (action.Type == "PLAY") {
                newHistory.AddPlay(action.PlayedHand!, action.Score);
            } else {
                newHistory.AddDiscard(new List<Card>(action.Cards));
            }
        }
        return newHistory;
    }
}