using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.TagCloud")]
    public class TagCloudMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable(
                "TagCloudRecord",
                table => table
                             .ContentPartRecord()
                             .Column("Buckets", DbType.Int32)
                             .Column("Slug", DbType.String)
                );

            ContentDefinitionManager.AlterPartDefinition(
                "TagCloudPart",
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition(
                "TagCloud",
                cfg => cfg
                           .WithPart("TagCloudPart")
                           .WithPart("CommonPart")
                           .WithPart("WidgetPart")
                           .WithSetting("Stereotype", "Widget")
                );
            return 1;
        }
    }
}