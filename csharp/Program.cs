using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Yuizumi.TexasHoldem
{
    internal class CommandLineException : Exception
    {
        internal CommandLineException(string message) : base(message)
        {
        }
    }

    internal static class Program
    {
        private static string Name
        {
            get {
                return Path.GetFileName(Environment.GetCommandLineArgs()[0]);
            }
        }

        private static int Main(string[] args)
        {
            try {
                Start(args);
                return 0;
            } catch (CommandLineException e) {
                Console.Error.WriteLine($"{Name}: {e.Message}");
                return 1;
            }
        }

        private static void Start(string[] args)
        {
            IEvaluator evaluator = ExactEvaluator.Evaluator;
            Table table = ParseTable(args);

            IReadOnlyList<Stats> stats = evaluator.ComputeStats(table);

            for (int i = 0; i < table.Pockets.Count; i++) {
                Console.WriteLine("[{0}] {1}: {2,7:P2} / {3,7:P2}",
                    i, table.Pockets[i], stats[i].WinProbability ,stats[i].LoseProbability);
            }
        }

        private static Table ParseTable(string[] args)
        {
            var pockets = new List<Pocket>();
            var community = new List<Card>();

            for (int i = 0; i < args.Length; i++) {
                if (args[i].StartsWith("+")) {
                    if (i != args.Length - 1)
                        throw new CommandLineException("Community must be specified last.");
                    community = ParseCards(args[i].Substring(1));
                    if (community.Count > 5)
                        throw new CommandLineException("Too many community cards.");
                } else {
                    var cards = ParseCards(args[i]);
                    if (cards.Count != 2)
                        throw new CommandLineException($"Not a valid pocket -- {args[i]}.");
                    pockets.Add(Pocket.Create(cards[0], cards[1]));
                }
            }

            if (pockets.Count < 2)
                throw new CommandLineException("Too few players.");

            var used = new HashSet<Card>();
            Card dupe = pockets.SelectMany(pocket => pocket).Concat(community).FirstOrDefault(
                card => !used.Add(card));
            if (dupe != null)
                throw new CommandLineException($"Card used multiple times -- {dupe}.");

            return new Table(pockets, community);
        }

        private static List<Card> ParseCards(string arg)
        {
            var cards = new List<Card>();
            for (int i = 0; i < arg.Length; i += 2) {
                if (i > arg.Length - 2)
                    throw new CommandLineException($"Invalid card -- {arg.Substring(i, 1)}.");
                Card card = Card.Parse(arg.Substring(i, 2));
                if (card == null)
                    throw new CommandLineException($"Invalid card -- {arg.Substring(i, 2)}.");
                cards.Add(card);
            }
            return cards;
        }
    }
}
