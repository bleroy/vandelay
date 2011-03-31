using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Vandelay.Meta {
    public class Migrations : DataMigrationImpl {

        public int Create() {
			SchemaBuilder.CreateTable("MetaRecord", table => table
				.ContentPartRecord()
				.Column("Keywords", DbType.String)
				.Column("Description", DbType.String)
			);

            ContentDefinitionManager.AlterPartDefinition(
                "MetaPart", cfg => cfg.Attachable());

            return 1;
        }
    }
}