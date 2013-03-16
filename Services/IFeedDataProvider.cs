using System.Web.Routing;
using Orchard;
using Orchard.ContentManagement;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Services {
    public interface IFeedDataProvider : IDependency {
        Feed GetFeed(string title, string format, RouteValueDictionary values);
        string GetKey(ContentItem item, string format);
        string GetKey(string title, string format, RouteValueDictionary values);
    }
}