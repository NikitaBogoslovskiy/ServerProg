using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPages.Data.Migrations
{
    public partial class CreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Testimonials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    ImgUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Occupation = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testimonials", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Testimonials",
                columns: new[] { "Id", "Description", "ImgUrl", "Name", "Occupation", "Title" },
                values: new object[] { 1, "They have got my project on time with the competition with a sed highly skilled, and experienced & professional team.", "uploads/testi_01.png", "James Fernando", "Manager of Racer", "Wonderful Support!" });

            migrationBuilder.InsertData(
                table: "Testimonials",
                columns: new[] { "Id", "Description", "ImgUrl", "Name", "Occupation", "Title" },
                values: new object[] { 2, "Explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you completed.", "uploads/testi_02.png", "Jacques Philips", "Designer", "Awesome Services!" });

            migrationBuilder.InsertData(
                table: "Testimonials",
                columns: new[] { "Id", "Description", "ImgUrl", "Name", "Occupation", "Title" },
                values: new object[] { 3, "The master-builder of human happines no one rejects, dislikes avoids pleasure itself, because it is very pursue pleasure.", "uploads/testi_03.png", "Venanda Mercy", "Newyork City", "Great & Talented Team!" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Testimonials");
        }
    }
}
