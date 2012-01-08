using NUnit.Framework;
using Orchard;
using Vandelay.Industries.Filters;
using Vandelay.Industries.Test.Stubs;

namespace Vandelay.Industries.Test {
    [TestFixture]
    public class RelativeUrlHtmlFilterTest {
        public RelativeUrlHtmlFilter Filter;
        public IWorkContextAccessor Wca;

        [SetUp]
        public void Setup() {
            Wca = new StubWorkContextAccessor("http://foo.com");
            Filter = new RelativeUrlHtmlFilter(Wca);
        }

        [Test]
        public void RelativeImgSrceGetsReplaced() {
            var html = @"<p><img src=""/foo/bar.png"" alt=""bar"">/foo/bar.png</p>";
            var transformed = Filter.ProcessContent(html, "html");
            Assert.AreEqual(@"<p><img src=""http://foo.com/foo/bar.png"" alt=""bar"">/foo/bar.png</p>", transformed);
        }

        [Test]
        public void AbsoluteImgSrcDoesNotGetReplaced() {
            var html = @"<p><img src=""http://bar.com/foo/bar.png"" alt=""bar"">/foo/bar.png</p>";
            var transformed = Filter.ProcessContent(html, "html");
            Assert.AreEqual(@"<p><img src=""http://bar.com/foo/bar.png"" alt=""bar"">/foo/bar.png</p>", transformed);
        }

        [Test]
        public void RelativeLinkHrefGetsReplaced() {
            var html = @"<p><a href=""/foo/bar"">/foo/bar</a></p>";
            var transformed = Filter.ProcessContent(html, "html");
            Assert.AreEqual(@"<p><a href=""http://foo.com/foo/bar"">/foo/bar</a></p>", transformed);
        }

        [Test]
        public void AbsoluteLinkHrefDoesNotGetReplaced() {
            var html = @"<p><a href=""http://bar.com/foo/bar"">/foo/bar</a></p>";
            var transformed = Filter.ProcessContent(html, "html");
            Assert.AreEqual(@"<p><a href=""http://bar.com/foo/bar"">/foo/bar</a></p>", transformed);
        }
    }
}
