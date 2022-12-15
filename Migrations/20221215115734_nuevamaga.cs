using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplicationplataformasdedesarrollo.Migrations
{
    /// <inheritdoc />
    public partial class nuevamaga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CajaAhorro",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cbu = table.Column<int>(type: "int", nullable: false),
                    saldo = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajaAhorro", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dni = table.Column<int>(type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    apellido = table.Column<string>(type: "varchar(50)", nullable: false),
                    mail = table.Column<string>(type: "varchar(50)", nullable: false),
                    intentosFallidos = table.Column<int>(type: "int", nullable: false),
                    bloqueado = table.Column<bool>(type: "bit", nullable: false),
                    password = table.Column<string>(type: "varchar(50)", nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Movimiento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    detalle = table.Column<string>(type: "varchar(255)", nullable: false),
                    monto = table.Column<double>(type: "float", nullable: false),
                    fecha = table.Column<DateTime>(type: "DateTime", nullable: false),
                    idCaja = table.Column<int>(name: "id_Caja", type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movimiento", x => x.id);
                    table.ForeignKey(
                        name: "FK_Movimiento_CajaAhorro_id_Caja",
                        column: x => x.idCaja,
                        principalTable: "CajaAhorro",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idusuario = table.Column<int>(name: "id_usuario", type: "int", nullable: false),
                    nombre = table.Column<string>(type: "varchar(50)", nullable: false),
                    monto = table.Column<double>(type: "float", nullable: false),
                    pagado = table.Column<bool>(type: "bit", nullable: false),
                    metodo = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pago", x => x.id);
                    table.ForeignKey(
                        name: "FK_Pago_Usuario_id_usuario",
                        column: x => x.idusuario,
                        principalTable: "Usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "PlazoFijo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    monto = table.Column<double>(type: "float", nullable: false),
                    fechaIni = table.Column<DateTime>(type: "dateTime", nullable: false),
                    fechaFin = table.Column<DateTime>(type: "dateTime", nullable: false),
                    tasa = table.Column<double>(type: "float", nullable: false),
                    pagado = table.Column<bool>(type: "bit", nullable: false),
                    idtitular = table.Column<int>(name: "id_titular", type: "int", nullable: false),
                    cbu = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlazoFijo", x => x.id);
                    table.ForeignKey(
                        name: "FK_PlazoFijo_Usuario_id_titular",
                        column: x => x.idtitular,
                        principalTable: "Usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Tarjeta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idtitular = table.Column<int>(name: "id_titular", type: "int", nullable: false),
                    numero = table.Column<int>(type: "int", nullable: false),
                    codigoV = table.Column<int>(type: "int", nullable: false),
                    limite = table.Column<double>(type: "float", nullable: false),
                    consumo = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarjeta", x => x.id);
                    table.ForeignKey(
                        name: "FK_Tarjeta_Usuario_id_titular",
                        column: x => x.idtitular,
                        principalTable: "Usuario",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UsuarioCaja",
                columns: table => new
                {
                    idUsuario = table.Column<int>(type: "int", nullable: false),
                    idCaja = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioCaja", x => new { x.idUsuario, x.idCaja });
                    table.ForeignKey(
                        name: "FK_UsuarioCaja_CajaAhorro_idCaja",
                        column: x => x.idCaja,
                        principalTable: "CajaAhorro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioCaja_Usuario_idUsuario",
                        column: x => x.idUsuario,
                        principalTable: "Usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "id", "apellido", "bloqueado", "dni", "intentosFallidos", "isAdmin", "mail", "nombre", "password" },
                values: new object[,]
                {
                    { 1, "Muzzio", false, 40009479, 0, true, "franco.muzzio@davinci.edu.ar", "Franco", "1234" },
                    { 2, "Piñeiro", false, 63309307, 0, true, "fiorella.piñeiro@davinci.edu.ar", "Fiorella", "1234" },
                    { 3, "Markauskas", false, 32677773, 0, true, "magalí.markauskas@davinci.edu.ar", "Magalí", "1234" },
                    { 4, "Sassano", false, 21035623, 0, true, "martín.sassano@davinci.edu.ar", "Martín", "1234" },
                    { 5, "Giudice", false, 23391008, 0, true, "agustín.giudice@davinci.edu.ar", "Agustín", "1234" },
                    { 6, "Maubert", false, 45686773, 0, true, "alexis.maubert@davinci.edu.ar", "Alexis", "1234" },
                    { 7, "Di Marco", false, 84355987, 0, false, "marcos.dimarco@davinci.edu.ar", "Marcos", "1234" },
                    { 8, "Gutierrez", false, 40563444, 2, false, "juliana.gutierrez@davinci.edu.ar", "Juliana", "1234" },
                    { 9, "Houseman", false, 30447163, 0, false, "ariana.houseman@davinci.edu.ar", "Ariana", "1234" },
                    { 10, "Poggi", false, 73026363, 1, false, "pedro.poggi@davinci.edu.ar", "Pedro", "1234" },
                    { 11, "Ramirez", true, 39440793, 0, false, "lazaro.ramirez@davinci.edu.ar", "Lazaro", "1234" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimiento_id_Caja",
                table: "Movimiento",
                column: "id_Caja");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_id_usuario",
                table: "Pago",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_PlazoFijo_id_titular",
                table: "PlazoFijo",
                column: "id_titular");

            migrationBuilder.CreateIndex(
                name: "IX_Tarjeta_id_titular",
                table: "Tarjeta",
                column: "id_titular");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioCaja_idCaja",
                table: "UsuarioCaja",
                column: "idCaja");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movimiento");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "PlazoFijo");

            migrationBuilder.DropTable(
                name: "Tarjeta");

            migrationBuilder.DropTable(
                name: "UsuarioCaja");

            migrationBuilder.DropTable(
                name: "CajaAhorro");

            migrationBuilder.DropTable(
                name: "Usuario");
        }
    }
}
