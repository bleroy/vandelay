using System.Collections.Generic;
using NUnit.Framework;

namespace Vandelay.Industries.Test {
    public static class AssertHelper {
        public static void AreEquivalent(IEnumerable<string> expected, IEnumerable<string> actual) {
            var lookup = new HashSet<string>(expected);
            foreach (var str in actual) {
                if (!lookup.Contains(str)) throw new AssertionException(string.Format("'{0}' not in the list.", str));
            }
            lookup = new HashSet<string>(actual);
            foreach (var str in expected) {
                if (!lookup.Contains(str)) throw new AssertionException(string.Format("'{0}' not found.", str));
            }
        }
    }
}
