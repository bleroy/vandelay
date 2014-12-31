using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.RemoteRss")]
    public class RemoteRssMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable(
                "RemoteRssPartRecord",
                table => table
                             .ContentPartRecord()
                             .Column("RemoteRssUrl", DbType.String)
                             .Column("CacheDuration", DbType.Int32)
                             .Column("ItemsToDisplay", DbType.Int32)
                );

            ContentDefinitionManager.AlterPartDefinition(
                typeof(RemoteRssPart).Name, cfg => cfg.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "RemoteRssWidget",
                cfg => cfg
                           .WithPart("RemoteRssPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );

            return 1;
        }
    }
}