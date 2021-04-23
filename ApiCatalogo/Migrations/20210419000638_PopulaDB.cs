using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiCatalogo.Migrations
{
    public partial class PopulaDB : Migration
    {
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES('Bebidas','http://www.macoratti.net/Imagens/1.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES('Lanches','http://www.macoratti.net/Imagens/2.jpg')");
            mb.Sql("INSERT INTO Categorias(Nome, ImagemUrl) VALUES('Sobremesas','http://www.macoratti.net/Imagens/3.jpg')");

            mb.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId)" +
                "VALUES('Coca-Cola Diet','Refrigerante de cola 350ml',5.45,'http://www.macoratti.net/Imagens/coca.jpg',50,now()," +
                "(SELECT CategoriaId FROM Categorias where Nome = 'Bebidas'))");
            mb.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId)" +
               "VALUES('Lanche de Atum','Lanche de Atum com Maionese',8.50,'http://www.macoratti.net/Imagens/atum.jpg',50,now()," +
               "(SELECT CategoriaId FROM Categorias where Nome = 'Lanches'))");
            mb.Sql("INSERT INTO Produtos(Nome, Descricao, Preco, ImagemUrl, Estoque, DataCadastro, CategoriaId)" +
               "VALUES('Pudim','Pudim de Leite Condensado',6.75,'http://www.macoratti.net/Imagens/pudim.jpg',50,now()," +
               "(SELECT CategoriaId FROM Categorias where Nome = 'Sobremesas'))");
        }

        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Produtos");
            mb.Sql("DELETE FROM Categorias");
        }
    }
}
