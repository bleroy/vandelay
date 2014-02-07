using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.Classy")]
    public class ClassyMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable(
                "CustomCssRecord",
                table => table
                             .ContentPartRecord()
                             .Column("CustomId", DbType.String)
                             .Column("CssClass", DbType.String)
                             .Column("Scripts", DbType.String)
                );

            ContentDefinitionManager.AlterPartDefinition("CustomCss", builder => builder.Attachable());
            return 1;
        }
    }
}