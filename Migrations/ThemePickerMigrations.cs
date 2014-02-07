using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.ThemePicker")]
    public class ThemePickerMigrations : DataMigrationImpl {

        public int Create() {
            SchemaBuilder.CreateTable("ThemePickerSettingsRecord",
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
    }
}