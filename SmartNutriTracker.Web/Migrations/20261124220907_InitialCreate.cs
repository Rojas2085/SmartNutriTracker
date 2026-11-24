using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartNutriTracker.Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Categoria = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Icono = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    PorcionReferencia = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    GramosPorcion = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Calorias = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    ProteinasGramos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CarbohidratosGramos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    GrasasGramos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SemaforoNutricional = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EsFavorito = table.Column<bool>(type: "INTEGER", nullable: false),
                    EsAptoQuioscoEscolar = table.Column<bool>(type: "INTEGER", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriasAcceso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Rol = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    EquipoAsignado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Exitoso = table.Column<bool>(type: "INTEGER", nullable: false),
                    Origen = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Detalle = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriasAcceso", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogsGlobalesSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nivel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    ModuloOrigen = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DetalleExcepcion = table.Column<string>(type: "TEXT", nullable: true),
                    RutaEndpoint = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    UsuarioEmail = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    FechaHora = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogsGlobalesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReglasNutricionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreRegla = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ClasificacionImc = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Objetivo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RangoEdadMin = table.Column<int>(type: "INTEGER", nullable: false),
                    RangoEdadMax = table.Column<int>(type: "INTEGER", nullable: false),
                    CaloriasMin = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    CaloriasMax = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    PorcentajeProteinas = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    PorcentajeCarbohidratos = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    PorcentajeGrasas = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    MetaVasosAgua = table.Column<int>(type: "INTEGER", nullable: false),
                    MetaMinutosActividad = table.Column<int>(type: "INTEGER", nullable: false),
                    GuiaNutricional = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Activa = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReglasNutricionales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreCompleto = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    EquipoAsignado = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlertasNutricionales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EstudianteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoAlerta = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    NivelSeveridad = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Atendida = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaAtencion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DocenteAtendioId = table.Column<int>(type: "INTEGER", nullable: true),
                    ObservacionesAtencion = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasNutricionales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertasNutricionales_Usuarios_DocenteAtendioId",
                        column: x => x.DocenteAtendioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlertasNutricionales_Usuarios_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenusDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NombreMenu = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    FechaMenu = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoComida = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EstadoVariante = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TotalCalorias = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    TotalProteinas = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    TotalCarbohidratos = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    TotalGrasas = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Semaforo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EsSaludable = table.Column<bool>(type: "INTEGER", nullable: false),
                    DetalleAlimentosResumen = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    EstudianteId = table.Column<int>(type: "INTEGER", nullable: true),
                    NutricionistaId = table.Column<int>(type: "INTEGER", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenusDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenusDiarios_Usuarios_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MenusDiarios_Usuarios_NutricionistaId",
                        column: x => x.NutricionistaId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NotificacionesInternas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioReceptorId = table.Column<int>(type: "INTEGER", nullable: false),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Mensaje = table.Column<string>(type: "TEXT", maxLength: 400, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    Leida = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaLectura = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesInternas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacionesInternas_Usuarios_UsuarioReceptorId",
                        column: x => x.UsuarioReceptorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilesEstudiantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    CodigoEstudiante = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    GradoSeccion = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sexo = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    PesoKg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TallaCm = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Imc = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    ClasificacionImc = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    ObjetivoNutricional = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilesEstudiantes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilesEstudiantes_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosHabitosDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EstudianteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VasosAgua = table.Column<int>(type: "INTEGER", nullable: false),
                    MililitrosAgua = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    HorasSueno = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    MinutosActividadFisica = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoActividadFisica = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    EstadoAnimo = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaUltimaModificacion = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosHabitosDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosHabitosDiarios_Usuarios_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesMenuAlimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MenuDiarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlimentoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Unidad = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SubtotalCalorias = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    SubtotalProteinas = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SubtotalCarbohidratos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    SubtotalGrasas = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesMenuAlimentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesMenuAlimentos_Alimentos_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesMenuAlimentos_MenusDiarios_MenuDiarioId",
                        column: x => x.MenuDiarioId,
                        principalTable: "MenusDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecomendacionesInteligentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EstudianteId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReglaNutricionalId = table.Column<int>(type: "INTEGER", nullable: true),
                    MenuSugeridoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    MensajePersonalizado = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CaloriasSugeridas = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    ProteinasGramos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CarbohidratosGramos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    GrasasGramos = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    VasosAguaSugeridos = table.Column<int>(type: "INTEGER", nullable: false),
                    MinutosEjercicioSugeridos = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaGeneracion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Leida = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecomendacionesInteligentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecomendacionesInteligentes_MenusDiarios_MenuSugeridoId",
                        column: x => x.MenuSugeridoId,
                        principalTable: "MenusDiarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecomendacionesInteligentes_ReglasNutricionales_ReglaNutricionalId",
                        column: x => x.ReglaNutricionalId,
                        principalTable: "ReglasNutricionales",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecomendacionesInteligentes_Usuarios_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialesAntropometricos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PerfilEstudianteId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaMedicion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PesoKg = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TallaCm = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ImcCalculado = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    ClasificacionImc = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    ObservacionesDocente = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialesAntropometricos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialesAntropometricos_PerfilesEstudiantes_PerfilEstudianteId",
                        column: x => x.PerfilEstudianteId,
                        principalTable: "PerfilesEstudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportesNutricionalesSemanales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PerfilEstudianteId = table.Column<int>(type: "INTEGER", nullable: false),
                    SemanaNumero = table.Column<int>(type: "INTEGER", nullable: false),
                    Anio = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PromedioCaloriasDiarias = table.Column<decimal>(type: "decimal(7,2)", nullable: false),
                    CumplimientoAguaPorcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    DiasActividadFisica = table.Column<int>(type: "INTEGER", nullable: false),
                    PromedioHorasSueno = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    NivelRiesgo = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    ObservacionesDocente = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    GeneradoAutomaticamente = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesNutricionalesSemanales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesNutricionalesSemanales_PerfilesEstudiantes_PerfilEstudianteId",
                        column: x => x.PerfilEstudianteId,
                        principalTable: "PerfilesEstudiantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumosAlimentosReales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistroHabitoDiarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlimentoId = table.Column<int>(type: "INTEGER", nullable: true),
                    DescripcionAlimento = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    TipoComida = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    PorcionesConsumidas = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CaloriasAproximadas = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    HoraConsumo = table.Column<TimeSpan>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumosAlimentosReales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumosAlimentosReales_Alimentos_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimentos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ConsumosAlimentosReales_RegistrosHabitosDiarios_RegistroHabitoDiarioId",
                        column: x => x.RegistroHabitoDiarioId,
                        principalTable: "RegistrosHabitosDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertasNutricionales_DocenteAtendioId",
                table: "AlertasNutricionales",
                column: "DocenteAtendioId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertasNutricionales_EstudianteId",
                table: "AlertasNutricionales",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosAlimentosReales_AlimentoId",
                table: "ConsumosAlimentosReales",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosAlimentosReales_RegistroHabitoDiarioId",
                table: "ConsumosAlimentosReales",
                column: "RegistroHabitoDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesMenuAlimentos_AlimentoId",
                table: "DetallesMenuAlimentos",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesMenuAlimentos_MenuDiarioId",
                table: "DetallesMenuAlimentos",
                column: "MenuDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialesAntropometricos_PerfilEstudianteId",
                table: "HistorialesAntropometricos",
                column: "PerfilEstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_MenusDiarios_EstudianteId",
                table: "MenusDiarios",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_MenusDiarios_NutricionistaId",
                table: "MenusDiarios",
                column: "NutricionistaId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesInternas_UsuarioReceptorId",
                table: "NotificacionesInternas",
                column: "UsuarioReceptorId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesEstudiantes_UsuarioId",
                table: "PerfilesEstudiantes",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RecomendacionesInteligentes_EstudianteId",
                table: "RecomendacionesInteligentes",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_RecomendacionesInteligentes_MenuSugeridoId",
                table: "RecomendacionesInteligentes",
                column: "MenuSugeridoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecomendacionesInteligentes_ReglaNutricionalId",
                table: "RecomendacionesInteligentes",
                column: "ReglaNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosHabitosDiarios_EstudianteId",
                table: "RegistrosHabitosDiarios",
                column: "EstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesNutricionalesSemanales_PerfilEstudianteId",
                table: "ReportesNutricionalesSemanales",
                column: "PerfilEstudianteId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertasNutricionales");

            migrationBuilder.DropTable(
                name: "AuditoriasAcceso");

            migrationBuilder.DropTable(
                name: "ConsumosAlimentosReales");

            migrationBuilder.DropTable(
                name: "DetallesMenuAlimentos");

            migrationBuilder.DropTable(
                name: "HistorialesAntropometricos");

            migrationBuilder.DropTable(
                name: "LogsGlobalesSistema");

            migrationBuilder.DropTable(
                name: "NotificacionesInternas");

            migrationBuilder.DropTable(
                name: "RecomendacionesInteligentes");

            migrationBuilder.DropTable(
                name: "ReportesNutricionalesSemanales");

            migrationBuilder.DropTable(
                name: "RegistrosHabitosDiarios");

            migrationBuilder.DropTable(
                name: "Alimentos");

            migrationBuilder.DropTable(
                name: "MenusDiarios");

            migrationBuilder.DropTable(
                name: "ReglasNutricionales");

            migrationBuilder.DropTable(
                name: "PerfilesEstudiantes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
