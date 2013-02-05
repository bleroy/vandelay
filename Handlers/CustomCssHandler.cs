using System;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.UI.Resources;
using Vandelay.Industries.Models;

namespace Vandelay.Industries.Handlers {
    [OrchardFeature("Vandelay.Classy")]
    public class CustomCssHandler : ContentHandler {
        private readonly Work<IResourceManager> _resourceManager;

        public CustomCssHandler(IRepository<CustomCssRecord> repository, Work<IResourceManager> resourceManager) {
            Filters.Add(StorageFilter.For(repository));
            _resourceManager = resourceManager;
        }

        protected override void BuildDisplayShape(BuildDisplayContext context) {
            if (context.DisplayType == "Detail" && context.ContentItem.Has(typeof(CustomCss))) {
                var customCss = context.ContentItem.As<CustomCss>();
                var classes = customCss.CssClass;
                if (!String.IsNullOrWhiteSpace(classes)) {
                    foreach (var cssClass in classes.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)) {
                        context.Shape.Classes.Add(cssClass);
                    }
                }
                var id = customCss.CustomId;
                if (!String.IsNullOrWhiteSpace(id)) {
                    context.Shape.Attributes.Add("id", id);
                }
                var scripts = (customCss.Scripts ?? String.Empty)
                    .Split(new[] {' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var script in scripts) {
                    if (script.EndsWith(".js", StringComparison.OrdinalIgnoreCase)) {
                        _resourceManager.Value.Include("script", script, null);
                    }
                    else {
                        _resourceManager.Value.Require("script", script);
                    }
                }
            }
            base.BuildDisplayShape(context);
        }
    }
}
