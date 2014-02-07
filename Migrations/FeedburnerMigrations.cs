using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Vandelay.Industries.Models;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.Feedburner")]
    public class FeedburnerMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable(
                "FeedburnerPartRecord",
                table => table
                             .ContentPartRecord()
                             .Column("FeedburnerUrl", DbType.String)
                );

            ContentDefinitionManager.AlterPartDefinition(
                typeof(FeedburnerPart).Name, cfg => cfg.Attachable());

            return 1;
        }
    }
}