using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriMarket.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReseedServiceCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM "ServiceCategories"
                WHERE "Name" NOT IN ('Hay Baling','Combine Harvesting','Spraying','Soil Preparation','Seeding','Mowing','Transport');

                INSERT INTO "ServiceCategories" ("Id", "Name", "Description")
                VALUES
                    ('a1b2c3d4-0001-0000-0000-000000000001', 'Hay Baling', 'Round and square baling services'),
                    ('a1b2c3d4-0002-0000-0000-000000000002', 'Combine Harvesting', 'Grain and cereal harvesting'),
                    ('a1b2c3d4-0003-0000-0000-000000000003', 'Spraying', 'Crop protection and fertilizer spraying'),
                    ('a1b2c3d4-0004-0000-0000-000000000004', 'Soil Preparation', 'Ploughing, discing, and cultivating'),
                    ('a1b2c3d4-0005-0000-0000-000000000005', 'Seeding', 'Precision and broadcast seeding'),
                    ('a1b2c3d4-0006-0000-0000-000000000006', 'Mowing', 'Grass and hay mowing services'),
                    ('a1b2c3d4-0007-0000-0000-000000000007', 'Transport', 'Agricultural cargo transport')
                ON CONFLICT ("Id") DO NOTHING;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
