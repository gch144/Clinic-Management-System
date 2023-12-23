using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth.Migrations
{
    /// <inheritdoc />
    public partial class doctorseduledabcdssssssss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorSchedules_DoctorScheduleId1",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorScheduleId1",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorScheduleId1",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "DoctorScheduleId",
                table: "Appointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorScheduleId",
                table: "Appointments",
                column: "DoctorScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorSchedules_DoctorScheduleId",
                table: "Appointments",
                column: "DoctorScheduleId",
                principalTable: "DoctorSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_DoctorSchedules_DoctorScheduleId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_DoctorScheduleId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "DoctorScheduleId",
                table: "Appointments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "DoctorScheduleId1",
                table: "Appointments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorScheduleId1",
                table: "Appointments",
                column: "DoctorScheduleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_DoctorSchedules_DoctorScheduleId1",
                table: "Appointments",
                column: "DoctorScheduleId1",
                principalTable: "DoctorSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
