using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.ProductApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert into products(Name, Price, Description, Stock, ImageURL, CategoryId)" +
                "values('Caderno',7.55, 'caderno', 10, 'caderno1.jpg',1)");

            mb.Sql("Insert into products(Name, Price, Description, Stock, ImageURL, CategoryId)" +
                "values('Caneta',2.50, 'caneta azul', 25, 'caneta1.jpg',1)");

            mb.Sql("Insert into products(Name, Price, Description, Stock, ImageURL, CategoryId)" +
                "values('Borracha',1.75, 'borracha branca', 15, 'borracha1.jpg',1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from products");
        }
    }
}
