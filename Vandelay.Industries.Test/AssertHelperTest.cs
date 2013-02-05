using NUnit.Framework;

namespace Vandelay.Industries.Test {
    [TestFixture]
    public class AssertHelperTest {
        [Test]
        public void Right() {
            AssertHelper.AreEquivalent(
                new [] {"foo", "bar", ""},
                new [] {"bar", "", "foo"}
                );
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(AssertionException), ExpectedMessage = "'baz' not in the list.")]
        public void TooMany() {
            AssertHelper.AreEquivalent(
                new[] { "foo", "bar" },
                new[] { "bar", "baz", "foo" }
                );
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(AssertionException), ExpectedMessage = "'baz' not found.")]
        public void TooFew() {
            AssertHelper.AreEquivalent(
                new[] { "bar", "baz", "foo" },
                new[] { "foo", "bar" }
                );
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(AssertionException), ExpectedMessage = "'ga' not in the list.")]
        public void Nawak() {
            AssertHelper.AreEquivalent(
                new[] { "bar", "baz", "foo" },
                new[] { "ga", "bu", "zo", "meu" }
                );
        }

        [Test]
        [ExpectedException(ExpectedException = typeof(AssertionException), ExpectedMessage = "'ga' not in the list.")]
        public void EmptyArrayContainsNothing() {
            AssertHelper.AreEquivalent(
                new string[0],
                new[] { "ga", "bu", "zo", "meu" }
                );
        }

        [Test]
        public void EmptyArrayEquivalentToEmptyArray() {
            AssertHelper.AreEquivalent(
                new string[0],
                new string[0]
                );
        }
    }
}
