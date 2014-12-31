using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries.Migrations {
    [OrchardFeature("Vandelay.CustomSort")]
    public class CustomSortMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable(
                "CustomSortRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Name"));

            SchemaBuilder.CreateTable(
                "CustomSortOrderRecord",
                table => table
                    .Column<int>("Id", column => column.NotNull())
                    .Column<int>("CustomSortRecord_Id")
                    .Column<int>("SortOrder"));

            SchemaBuilder.CreateForeignKey("OrderToSort", "Vandelay.Industries",
                "CustomSortOrderRecord", new[] {"CustomSortRecord_Id"},
                "CustomSortRecord", new[] {"Id"});

            SchemaBuilder.AlterTable("CustomSortOrderRecord", table => table
                .CreateIndex("SortOrderIndex", new[] { "SortOrder" }));

            return 1;
        }
    }
}