using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Betauer.Core.Deck;
using Betauer.Core.Deck.Hands;
using static System.Int32;

namespace Betauer.Core.Examples;

public class SolitaireConsoleDemo {

    public readonly bool _autoPlay;
    public readonly List<GameRun> GameRuns = new();
    public GameRun CurrentRun;
    public GameHandler GameHandler;
    public PokerGameConfig Config;
    public PokerHandsManager PokerHandsManager;
    private readonly AutoPlayer autoPlayer = new AutoPlayer();
    private AutoPlayer.AutoPlayDecision? currentDecision;

    public SolitaireConsoleDemo(bool autoPlay = false) {
        _autoPlay = autoPlay;
    }

    public static void Main() {
        Console.WriteLine("Auto-play mode? (Y/N)");
        var autoPlay = Console.ReadLine()?.ToUpper() == "Y";
        new SolitaireConsoleDemo(autoPlay).Play();
    }

    public void Play() {
        InitializeFirstRun();

        while (GameHandler.State.Level < GameHandler.Config.MaxLevel) {
            while (!GameHandler.IsGameOver()) {
                currentDecision = autoPlayer.GetNextAction(GameHandler);
                DisplayGameState();

                if (_autoPlay) {
                    ProcessAutoPlay();
                    //Thread.Sleep(1000);
                } else {
                    if (ProcessUserInput() == true) {
                        return; // El usuario quiere salir
                    }
                }
            }

            if (GameHandler.IsWon()) {
                DisplayWinScreen();

                if (GameHandler.State.Level + 1 < GameHandler.Config.MaxLevel) {
                    // Preparamos el siguiente nivel
                    InitializeGame(GameHandler.State.Level + 1);

                    Console.WriteLine($"\nAdvancing to level {GameHandler.State.Level + 1}! New target: {GameHandler.State.LevelScore}");
                    if (!_autoPlay) {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                }
            } else {
                // El juego terminó sin alcanzar la puntuación
                DisplayGameOverScreen();
                if (!_autoPlay && !PlayAgain()) {
                    return;
                }
                InitializeNextRun();
            }
        }

        DisplayFinalVictoryScreen();
    }

    private void InitializeFirstRun() {
        Config = new PokerGameConfig();
        PokerHandsManager = new PokerHandsManager();
        PokerHandsManager.RegisterBasicPokerHands();
        var seed = 0;
        var level = 0;
        if (!_autoPlay) {
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
            if (TryParse(input, out level)) {
            }
            Console.WriteLine($"Level: {level}");
            Thread.Sleep(600);
        }
        
        CurrentRun = new GameRun(0, Config, PokerHandsManager, seed);
        GameRuns.Add(CurrentRun);
        InitializeGame(level);
    }

    private void InitializeNextRun() {
        var nextId = CurrentRun.Id + 1;
        CurrentRun = new GameRun(nextId, Config, PokerHandsManager, nextId);
        GameRuns.Add(CurrentRun);
        InitializeGame(0);
    }

    private void InitializeGame(int level) {
        GameHandler = CurrentRun.CreateGameHandler(level);
        GameHandler.DrawCards();
    }

    private void DisplayGameState() {
        var state = GameHandler.State;
        Console.Clear();

        // Display runs history
        if (GameRuns.Count > 0) {
            Console.WriteLine("=== Previous Runs ===");
            // foreach (var run in gameRuns.OrderByDescending(r => r.StartTime)) {
            foreach (var run in GameRuns) {
                Console.WriteLine(run);
                foreach (var gameState in run.GameStates) {
                    Console.Write($"  Level {gameState.Level + 1} | ");
                    foreach (var action in gameState.History.GetHistory()) {
                        if (action.Type == PlayHistory.PlayedActionType.Play)
                            Console.Write($"Play #{action.Id + 1}: {action.PlayedHand?.Name} ({string.Join(", ", action.Cards)}) (Score +{action.HandScore}: {action.GameScore}/{action.TotalScore}) | ");
                        else
                            Console.Write($"Discard #{action.Id + 1}: {string.Join(" ", action.Cards)} | ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.WriteLine("=== Stats ===");
            if (GameRuns.Count >= 2) {
                var gameRunsWithoutLast = GameRuns.Take(GameRuns.Count - 1);
                var runsWithStates = gameRunsWithoutLast.Where(run => run.GameStates.Count > 0).ToList();
    
                if (runsWithStates.Any()) {
                    var minLevelWon = runsWithStates.Min(run => run.GameStates.Last().Level);
                    var maxLevelWon = runsWithStates.Max(run => run.GameStates.Last().Level);

                    // Encontrar los runs que tienen el nivel mínimo y máximo para obtener sus seeds
                    var runWithMinLevel = runsWithStates.First(run => run.GameStates.Last().Level == minLevelWon);
                    var runWithMaxLevel = runsWithStates.First(run => run.GameStates.Last().Level == maxLevelWon);
                    var minLevelState = runWithMinLevel.GameStates.Last();
                    var maxLevelState = runWithMaxLevel.GameStates.Last();

                    Console.WriteLine($"- Min level won: {(minLevelWon + 1)} (seed {minLevelState.Seed}) | Max level won: {(maxLevelWon + 1)} (seed {maxLevelState.Seed})]");
    
                    // Añadimos el resumen de runs por nivel
                    var runsByLevel = runsWithStates
                        .GroupBy(run => run.GameStates.Last().Level)
                        .OrderBy(group => group.Key)
                        .ToDictionary(group => group.Key, group => group.Count());
    
                    Console.WriteLine("=== Runs distribution by level ===");
                    foreach (var (level, count) in runsByLevel) {
                        Console.WriteLine($"Level {level + 1}: {count} run{(count > 1 ? "s" : "")}");
                    }
                }
            }
        }
        Console.WriteLine($"=== Solitaire Poker - Seed: {state.Seed} - Level {state.Level + 1} ===");
        Console.WriteLine($"Score: {state.Score}/{state.LevelScore} | Hand {state.HandsPlayed + 1}/{GameHandler.Config.MaxHands} | Discards: {state.Discards}/{GameHandler.Config.MaxDiscards}");
        DisplayYourHand();
    }

    private void DisplayWinScreen() {
        if (_autoPlay) return;
        Console.Clear();
        Console.WriteLine("=== WINNER! ===");
        Console.WriteLine($"Congratulations! You've reached {GameHandler.State.Score} points!");
        Console.WriteLine($"Target was: {GameHandler.State.LevelScore}");
    }

    private void DisplayGameOverScreen() {
        if (_autoPlay) return;
        Console.Clear();
        Console.WriteLine("=== GAME OVER ===");
        Console.WriteLine($"Final Score: {GameHandler.State.Score}");
        Console.WriteLine($"Target Score: {GameHandler.State.LevelScore}");
        Console.WriteLine($"You needed {GameHandler.State.LevelScore - GameHandler.State.Score} more points to win");
    }

    private void DisplayFinalVictoryScreen() {
        if (_autoPlay) return;
        Console.Clear();
        Console.WriteLine("=== CONGRATULATIONS! ===");
        Console.WriteLine("You've completed all levels!");
        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private bool PlayAgain() {
        if (_autoPlay) return true;
        Console.WriteLine("\nPlay again? (Y/N)");
        return Console.ReadLine()?.ToUpper() == "Y";
    }

    private void DisplayYourHand() {
        var cards = GameHandler.State.CurrentHand;
        var groupedCards = cards
            .GroupBy(c => c.Suit)
            .OrderBy(g => g.Key)
            .ToList();

        Console.Write("Your hand | By suit: ");
        foreach (var group in groupedCards) {
            var sortedCards = group.OrderByDescending(c => c.Rank);
            Console.Write($"{string.Join(" ", sortedCards)} | ");
        }
        Console.Write("| By rank: ");
        Console.WriteLine($"{string.Join(" ", cards.OrderByDescending(c => c.Rank))}");
    }

    private void DisplayPotentialHands() {
        Console.WriteLine("\nPossible hands you can play:");
        int i = 0;
        for (; i < currentDecision.PossibleHands.Count; i++) {
            Console.WriteLine($"{i + 1}: {currentDecision.PossibleHands[i]}: +{GameHandler.CalculateScore(currentDecision.PossibleHands[i])}");
        }
        var currentBestHand = currentDecision.PossibleHands[0];

        var bestHandCards = currentDecision.PossibleHands[0].Cards;

        foreach (var discards in currentDecision.DiscardOptions.Discards) {
            i++;
            var discardBestHand = discards.GetBestHand();

            var breaksYouHand = discards.CardsToDiscard.Any(bestHandCards.Contains);

            Console.WriteLine($"{i}. Discarding {discards.CardsToDiscard.Count}: {string.Join(" ", discards.CardsToDiscard)}, keeping: {string.Join(" ", discards.CardsToKeep)} | Score: {discardBestHand.PotentialScore:F2}{(breaksYouHand ? " | Breaks your current hand!" : "")}");
            var handsByScore = discards.HandOccurrences
                .OrderByDescending(kv => kv.Value.PotentialScore);

            foreach (var (handType, stats) in handsByScore) {
                Console.WriteLine($"   - {handType.Name,-20} {stats.AvgScore:0000} x {stats.Probability:00.00%} = {stats.PotentialScore:000.00} {(stats.Probability < Risk ? "[!]" : "[ ]")} {(handType == currentBestHand.GetType() ? "same as your hand" : "")}");
            }
        }
        Console.WriteLine($"Analysis time: {currentDecision.DiscardOptions.ElapsedTime.TotalSeconds:F3} seconds");
        Console.WriteLine($"Total simulations: {currentDecision.DiscardOptions.TotalSimulations:N0}/{currentDecision.DiscardOptions.TotalCombinations:N0} ({(float)currentDecision.DiscardOptions.TotalSimulations / currentDecision.DiscardOptions.TotalCombinations:0%})");
    }

    private bool ProcessUserInput() {
        currentDecision = autoPlayer.GetNextAction(GameHandler);
        DisplayPotentialHands();

        Console.WriteLine("\nOptions:");
        Console.WriteLine("1-N: Play hand/discard from the list");
        Console.WriteLine("M: Make your own hand by selecting cards");
        Console.WriteLine("D: Discard cards");
        Console.WriteLine("A: Auto play: " + GetAutoPlay());
        Console.WriteLine("Q: Quit game");

        var option = Console.ReadLine()?.ToUpper();

        if (option == "Q") {
            return true;
        }

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
            if (choice <= currentDecision.PossibleHands.Count) {
                var hand = currentDecision.PossibleHands[choice - 1];
                ProcessHand(hand.Cards);
            } else {
                choice -= currentDecision.PossibleHands.Count;
                var cardsToDiscard = currentDecision.DiscardOptions.Discards[choice - 1].CardsToDiscard;
                ProcessDiscard(cardsToDiscard);
            }
        } else {
            Console.WriteLine("Invalid option! (press any key to continue)");
            Console.ReadKey();
        }
        return false;
    }

    private string GetAutoPlay() {
        if (currentDecision == null) throw new InvalidOperationException("currentDecision is null");
        var bestHandIfDiscard = currentDecision.DiscardOption?.GetBestHand();
        if (currentDecision.HandToPlay != null && currentDecision.ShouldPlay) {
            return $"[Play {currentDecision.HandToPlay}] +{GameHandler.CalculateScore(currentDecision.HandToPlay)} | Reason: {currentDecision.Reason}";
        } else {
            return $"[Discard {string.Join(", ", currentDecision.DiscardOption!.CardsToDiscard)}] score {bestHandIfDiscard!.AvgScore:0} x {bestHandIfDiscard!.Probability:0.0%} = {bestHandIfDiscard!.PotentialScore:0.0} | Reason: {currentDecision.Reason}";
        }
    }

    private void ProcessAutoPlay() {
        if (currentDecision == null) throw new InvalidOperationException("currentDecision is null");
        if (currentDecision.HandToPlay != null && currentDecision.ShouldPlay) {
            var extraCards = GameHandler.Config.MaxHandSizeToPlay - currentDecision.HandToPlay.Cards.Count;
            if (extraCards > 0) {
                // If the current hand to play is smaller than the max size, add extra cards just to use them as discard
                var noPlayedCards = GameHandler.State.CurrentHand.Except(currentDecision.HandToPlay.Cards);
                var extraCardsToDiscard = noPlayedCards.OrderBy(i => i.Rank).Take(extraCards).ToList();
                IReadOnlyList<Card> newPokerHand = [..currentDecision.HandToPlay.Cards, ..extraCardsToDiscard];
                ProcessHand(newPokerHand);
            } else {
                ProcessHand(currentDecision.HandToPlay.Cards);
            }
        } else {
            ProcessDiscard(currentDecision.DiscardOption!.CardsToDiscard);
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

        Console.WriteLine($"Played {result.Hand?.Name}: {string.Join(", ", hand)}. Scored: +{result.Score} ({GameHandler.State.Score}/{GameHandler.State.LevelScore})");

        if (GameHandler.IsDrawPending()) {
            GameHandler.DrawCards();
            DisplayYourHand();
        }
        if (!_autoPlay) {
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
        Console.WriteLine($"Discarded: {string.Join(", ", cardsToDiscard)}");
        var result = GameHandler.Discard(cardsToDiscard);
        Console.WriteLine($"* Cards discarded. Remaining discards: {GameHandler.Config.MaxDiscards - GameHandler.State.Discards}");
        GameHandler.DrawCards();
        DisplayYourHand();
        if (!_autoPlay) {
            Console.WriteLine("(press any key to continue)");
            Console.ReadKey();
        }
    }

    private const float Risk = 0.7f;
}