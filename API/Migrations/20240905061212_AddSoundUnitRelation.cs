using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class AddSoundUnitRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sounds_SoundUnits_SoundUnitId",
                table: "Sounds");

            migrationBuilder.AlterColumn<int>(
                name: "SoundUnitId",
                table: "Sounds",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sounds_SoundUnits_SoundUnitId",
                table: "Sounds",
                column: "SoundUnitId",
                principalTable: "SoundUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sounds_SoundUnits_SoundUnitId",
                table: "Sounds");

            migrationBuilder.AlterColumn<int>(
                name: "SoundUnitId",
                table: "Sounds",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Sounds_SoundUnits_SoundUnitId",
                table: "Sounds",
                column: "SoundUnitId",
                principalTable: "SoundUnits",
                principalColumn: "Id");
        }
    }
}
