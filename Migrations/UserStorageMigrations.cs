using Orchard.Data.Migration;
using Orchard.Environment.Extensions;

namespace Vandelay.Industries {
    [OrchardFeature("Vandelay.UserStorage")]
    public class UserStorageMigrations : DataMigrationImpl {
        public int Create() {
            SchemaBuilder.CreateTable("UserStorageRecord",
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("UserName", column => column.NotNull().WithDefault(""))
                    .Column<string>("Folder", column => column.NotNull().WithDefault(""))
                    .Column<string>("FileName", column => column.NotNull().WithDefault(""))
                    .Column<string>("Contents", column => column.Unlimited())
                    .Column<int>("Size", column => column.NotNull().WithDefault(0))
                );
            SchemaBuilder.AlterTable("UserStorageRecord",
                table => table
                    .CreateIndex("IX_VandelayUserStorage", 
                        "UserName", "Folder", "FileName")
                );

            return 1;
        }
    }
}