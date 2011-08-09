using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.Favicon")]
    public class FaviconMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable(
                "FaviconSettingsPartRecord",
                table => table
                             .ContentPartRecord()
                             .Column<string>("FaviconUrl")
                );
            return 1;
        }
    }
}