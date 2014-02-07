using System.Data;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.Industries")]
    public class MetaMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("MetaRecord",
                                      table => table
                                                   .ContentPartRecord()
                                                   .Column("Keywords", DbType.String)
                                                   .Column("Description", DbType.String)
                );

            ContentDefinitionManager.AlterPartDefinition(
                "MetaPart", cfg => cfg.Attachable());

            SchemaBuilder.CreateTable("SettingsRecord",
                                      table => table
                                                   .Column<int>("Id", column => column.PrimaryKey().Identity())
                                                   .Column<string>("RuleType", column => column.NotNull().WithDefault(""))
                                                   .Column<string>("Name", column => column.NotNull().WithDefault(""))
                                                   .Column<string>("Criterion", column => column.NotNull().WithDefault(""))
                                                   .Column<string>("Theme", column => column.NotNull().WithDefault(""))
                                                   .Column<int>("Priority", column => column.NotNull().WithDefault(10))
                                                   .Column<string>("Zone", column => column.NotNull().WithDefault(""))
                                                   .Column<string>("Position", column => column.NotNull().WithDefault(""))
                );

            return 1;
        }

        public int UpdateFrom1() {
            SchemaBuilder.AlterTable("MetaRecord",
                                     table => {
                                         table.AlterColumn("Description", column => column.WithType(DbType.String).Unlimited());
                                         table.AlterColumn("Keywords", column => column.WithType(DbType.String).Unlimited());
                                     });
            return 2;
        }
    }
}