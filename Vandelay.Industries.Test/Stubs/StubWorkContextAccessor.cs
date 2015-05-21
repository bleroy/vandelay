using System;
using System.Collections.Generic;
using System.Web;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Settings;

namespace Vandelay.Industries.Test.Stubs {
    public class StubWorkContextAccessor : IWorkContextAccessor {
        private readonly WorkContext _workContext;

        public StubWorkContextAccessor(string baseUrl) {
            _workContext = new WorkContextImpl(baseUrl);
        }

        public class WorkContextImpl : WorkContext {
            private readonly Dictionary<string, object> _contextDictonary;

            public delegate void MyInitMethod(WorkContextImpl workContextImpl);

            public WorkContextImpl(string baseUrl) {
                _contextDictonary = new Dictionary<string, object>();
                var ci = new ContentItem();
                ci.Weld(new StubSite(baseUrl));
                CurrentSite = ci.As<ISite>();
            }

            public class StubSite : ContentPart, ISite {
                private string _baseUrl;

                public StubSite(string baseUrl) {
                    _baseUrl = baseUrl;
                }

                public string PageTitleSeparator {
                    get { throw new NotImplementedException(); }
                }

                public string SiteName {
                    get { throw new NotImplementedException(); }
                }

                public string SiteSalt {
                    get { throw new NotImplementedException(); }
                }

                public string SuperUser {
                    get { throw new NotImplementedException(); }
                }

                public string HomePage {
                    get { throw new NotImplementedException(); }
                    set { throw new NotImplementedException(); }
                }

                public string SiteCulture {
                    get { throw new NotImplementedException(); }
                    set { throw new NotImplementedException(); }
                }

                public string SiteCalendar { get; set; }

                public ResourceDebugMode ResourceDebugMode {
                    get { throw new NotImplementedException(); }
                    set { throw new NotImplementedException(); }
                }

                public bool UseCdn { get; set; }

                public int PageSize{
                    get { throw new NotImplementedException(); }
                    set { throw new NotImplementedException(); }
                }

                public int MaxPageSize { get; set; }
                public int MaxPagedCount { get; set; }

                public string BaseUrl { get { return _baseUrl; } }

                public string SiteTimeZone { get; set; }
            }

            public override T Resolve<T>() {
                throw new NotImplementedException();
            }

            public override bool TryResolve<T>(out T service) {
                throw new NotImplementedException();
            }

            public override T GetState<T>(string name) {
                return (T) _contextDictonary[name];
            }

            public override void SetState<T>(string name, T value) {
                _contextDictonary[name] = value;
            }
        }

        public WorkContext GetContext(HttpContextBase httpContext) {
            return _workContext;
        }

        public IWorkContextScope CreateWorkContextScope(HttpContextBase httpContext) {
            throw new NotSupportedException();
        }

        public WorkContext GetContext() {
            return _workContext;
        }

        public IWorkContextScope CreateWorkContextScope() {
            throw new NotSupportedException();
        }
    }
}
