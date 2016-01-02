using System;
using System.Collections.Generic;
using System.Linq;

namespace Yuizumi.TexasHoldem
{
    public static class VerifyArg
    {
        public static void True(bool condition, string name, string message)
        {
            if (!condition) throw new ArgumentException(message, name);
        }

        public static void NotNull<T>(T arg, string name)
            where T : class
        {
            if (arg == null) throw new ArgumentNullException(name);
        }

        public static void Count<T>(IEnumerable<T> arg, string name, int count)
        {
            NotNull(arg, name);
            True(arg.Count() == count, name, $"Collection must contain exactly {count} items.");
        }

        public static void Nonempty<T>(IEnumerable<T> arg, string name)
            where T : class
        {
            NotNull(arg, name);
            True(arg.Any(), name, "Collection must not be empty.");
        }

        public static void NoneNull<T>(IEnumerable<T> arg, string name)
            where T : class
        {
            NotNull(arg, name);
            True(!arg.Contains(null), name, "Collection must not contain null.");
        }
    }
}
