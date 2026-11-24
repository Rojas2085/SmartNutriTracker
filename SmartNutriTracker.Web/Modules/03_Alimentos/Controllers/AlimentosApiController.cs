using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartNutriTracker.Web.Core.Data;
using SmartNutriTracker.Web.Modules.Alimentos.Models;

namespace SmartNutriTracker.Web.Modules.Alimentos.Controllers;

[ApiController]
[Route("api/alimentos")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthSchemes)]
public class AlimentosApiController : ControllerBase
{
    private const string AuthSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}";
    private readonly AppDbContext _context;
    private readonly ILogger<AlimentosApiController> _logger;

    public AlimentosApiController(AppDbContext context, ILogger<AlimentosApiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lista alimentos con búsqueda y filtros por categoría (Todos, Lácteos, Carnes, Verduras, Frutas, Saludables, Favoritos).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListarAlimentos(
        [FromQuery] string? buscar,
        [FromQuery] string? categoria,
        [FromQuery] bool? soloFavoritos,
        [FromQuery] bool? soloSaludables)
    {
        var query = _context.Alimentos.Where(a => a.Activo).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var b = buscar.Trim().ToLower();
            query = query.Where(a => a.Nombre.ToLower().Contains(b) || a.Categoria.ToLower().Contains(b));
        }

        if (!string.IsNullOrWhiteSpace(categoria) && !categoria.Equals("Todos", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(a => a.Categoria.ToLower() == categoria.ToLower());
        }

        if (soloFavoritos == true)
        {
            query = query.Where(a => a.EsFavorito);
        }

        if (soloSaludables == true)
        {
            query = query.Where(a => a.SemaforoNutricional == "Verde");
        }

        var list = await query.OrderBy(a => a.Nombre).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerAlimento(int id)
    {
        var alimento = await _context.Alimentos.FindAsync(id);
        if (alimento == null || !alimento.Activo)
            return NotFound(new { mensaje = "Alimento no encontrado." });

        return Ok(alimento);
    }

    /// <summary>
    /// Registra un nuevo alimento en el catálogo con semáforo calculado automáticamente (HU-14).
    /// </summary>
    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> RegistrarAlimento([FromBody] Alimento model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Auto cálculo de semáforo si no viene asignado
        if (string.IsNullOrWhiteSpace(model.SemaforoNutricional))
        {
            model.SemaforoNutricional = CalcularSemaforo(model.Calorias, model.GrasasGramos);
        }

        if (string.IsNullOrWhiteSpace(model.Icono))
        {
            model.Icono = AsignarIcono(model.Categoria);
        }

        model.FechaCreacion = DateTime.UtcNow;
        model.Activo = true;

        await _context.Alimentos.AddAsync(model);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Alimento registrado: {Nombre} ({Calorias} kcal)", model.Nombre, model.Calorias);

        return CreatedAtAction(nameof(ObtenerAlimento), new { id = model.Id }, model);
    }

    /// <summary>
    /// Actualiza un alimento existente (HU-15).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> ActualizarAlimento(int id, [FromBody] Alimento model)
    {
        var existing = await _context.Alimentos.FindAsync(id);
        if (existing == null || !existing.Activo)
            return NotFound(new { mensaje = "Alimento no encontrado." });

        existing.Nombre = model.Nombre;
        existing.Categoria = model.Categoria;
        existing.PorcionReferencia = model.PorcionReferencia;
        existing.GramosPorcion = model.GramosPorcion;
        existing.Calorias = model.Calorias;
        existing.ProteinasGramos = model.ProteinasGramos;
        existing.CarbohidratosGramos = model.CarbohidratosGramos;
        existing.GrasasGramos = model.GrasasGramos;
        existing.SemaforoNutricional = model.SemaforoNutricional ?? CalcularSemaforo(model.Calorias, model.GrasasGramos);
        existing.Icono = !string.IsNullOrWhiteSpace(model.Icono) ? model.Icono : AsignarIcono(model.Categoria);
        existing.EsAptoQuioscoEscolar = model.EsAptoQuioscoEscolar;
        existing.Descripcion = model.Descripcion;

        await _context.SaveChangesAsync();
        return Ok(existing);
    }

    /// <summary>
    /// Alterna el estado de favorito de un alimento (★ Favoritos).
    /// </summary>
    [HttpPost("{id}/favorito")]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> ToggleFavorito(int id)
    {
        var alimento = await _context.Alimentos.FindAsync(id);
        if (alimento == null || !alimento.Activo)
            return NotFound();

        alimento.EsFavorito = !alimento.EsFavorito;
        await _context.SaveChangesAsync();

        return Ok(new { id = alimento.Id, esFavorito = alimento.EsFavorito });
    }

    /// <summary>
    /// Elimina lógicamente un alimento (HU-15).
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> EliminarAlimento(int id)
    {
        var alimento = await _context.Alimentos.FindAsync(id);
        if (alimento == null)
            return NotFound(new { mensaje = "Alimento no encontrado." });

        alimento.Activo = false;
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Alimento eliminado correctamente." });
    }

    // =========================================================================
    // ENDPOINTS DE MENÚS DIARIOS (HU-16, HU-17)
    // =========================================================================

    [HttpGet("menus")]
    public async Task<IActionResult> ListarMenus()
    {
        var menus = await _context.MenusDiarios
            .Include(m => m.Items)
                .ThenInclude(i => i.Alimento)
            .OrderByDescending(m => m.FechaMenu)
            .ToListAsync();

        return Ok(menus);
    }

    [HttpGet("menus/{id}")]
    public async Task<IActionResult> ObtenerMenu(int id)
    {
        var menu = await _context.MenusDiarios
            .Include(m => m.Items)
                .ThenInclude(i => i.Alimento)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
            return NotFound(new { mensaje = "Menú no encontrado." });

        return Ok(menu);
    }

    [HttpPost("menus")]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> GuardarMenu([FromBody] GuardarMenuDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NombreMenu))
            return BadRequest(new { mensaje = "El nombre del menú es obligatorio." });

        var menu = new MenuDiario
        {
            NombreMenu = dto.NombreMenu,
            FechaMenu = dto.FechaMenu ?? DateTime.Today,
            TipoComida = dto.TipoComida ?? "Desayuno",
            EstadoVariante = dto.EstadoVariante ?? "Nuevo",
            TotalCalorias = dto.TotalCalorias,
            TotalProteinas = dto.TotalProteinas,
            TotalCarbohidratos = dto.TotalCarbohidratos,
            TotalGrasas = dto.TotalGrasas,
            Semaforo = dto.Semaforo ?? "Verde",
            EsSaludable = dto.TotalCalorias <= 550,
            EstudianteId = dto.EstudianteId,
            FechaCreacion = DateTime.UtcNow
        };

        var nombresAlimentos = new List<string>();

        if (dto.Items != null && dto.Items.Any())
        {
            foreach (var item in dto.Items)
            {
                var alim = await _context.Alimentos.FindAsync(item.AlimentoId);
                if (alim != null)
                {
                    nombresAlimentos.Add(alim.Nombre);
                    menu.Items.Add(new DetalleMenuAlimento
                    {
                        AlimentoId = alim.Id,
                        Cantidad = item.Cantidad,
                        Unidad = item.Unidad ?? "g",
                        SubtotalCalorias = item.SubtotalCalorias,
                        SubtotalProteinas = item.SubtotalProteinas,
                        SubtotalCarbohidratos = item.SubtotalCarbohidratos,
                        SubtotalGrasas = item.SubtotalGrasas
                    });
                }
            }
        }

        menu.DetalleAlimentosResumen = string.Join(", ", nombresAlimentos);
        if (menu.DetalleAlimentosResumen.Length > 290)
            menu.DetalleAlimentosResumen = menu.DetalleAlimentosResumen.Substring(0, 287) + "...";

        await _context.MenusDiarios.AddAsync(menu);
        await _context.SaveChangesAsync();

        return Ok(new { exito = true, mensaje = "¡Menú guardado con éxito!", menuId = menu.Id });
    }

    [HttpPut("menus/{id}")]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> ActualizarMenu(int id, [FromBody] GuardarMenuDto dto)
    {
        var menu = await _context.MenusDiarios
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
            return NotFound(new { mensaje = "Menú no encontrado." });

        menu.NombreMenu = dto.NombreMenu;
        menu.FechaMenu = dto.FechaMenu ?? menu.FechaMenu;
        menu.TipoComida = dto.TipoComida ?? menu.TipoComida;
        menu.TotalCalorias = dto.TotalCalorias;
        menu.TotalProteinas = dto.TotalProteinas;
        menu.TotalCarbohidratos = dto.TotalCarbohidratos;
        menu.TotalGrasas = dto.TotalGrasas;
        menu.Semaforo = dto.Semaforo ?? "Verde";
        menu.EsSaludable = dto.TotalCalorias <= 550;
        menu.EstudianteId = dto.EstudianteId;

        _context.DetallesMenuAlimentos.RemoveRange(menu.Items);
        menu.Items.Clear();

        var nombresAlimentos = new List<string>();
        if (dto.Items != null)
        {
            foreach (var item in dto.Items)
            {
                var alim = await _context.Alimentos.FindAsync(item.AlimentoId);
                if (alim != null)
                {
                    nombresAlimentos.Add(alim.Nombre);
                    menu.Items.Add(new DetalleMenuAlimento
                    {
                        AlimentoId = alim.Id,
                        Cantidad = item.Cantidad,
                        Unidad = item.Unidad ?? "g",
                        SubtotalCalorias = item.SubtotalCalorias,
                        SubtotalProteinas = item.SubtotalProteinas,
                        SubtotalCarbohidratos = item.SubtotalCarbohidratos,
                        SubtotalGrasas = item.SubtotalGrasas
                    });
                }
            }
        }

        menu.DetalleAlimentosResumen = string.Join(", ", nombresAlimentos);
        if (menu.DetalleAlimentosResumen.Length > 290)
            menu.DetalleAlimentosResumen = menu.DetalleAlimentosResumen.Substring(0, 287) + "...";

        await _context.SaveChangesAsync();
        return Ok(new { exito = true, mensaje = "¡Menú actualizado con éxito!", menuId = menu.Id });
    }

    [HttpPost("menus/{id}/clonar")]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> ClonarMenu(int id)
    {
        var original = await _context.MenusDiarios
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (original == null)
            return NotFound(new { mensaje = "Menú original no encontrado." });

        var clon = new MenuDiario
        {
            NombreMenu = $"{original.NombreMenu} (Copia)",
            FechaMenu = DateTime.Today,
            TipoComida = original.TipoComida,
            EstadoVariante = "Nuevo",
            TotalCalorias = original.TotalCalorias,
            TotalProteinas = original.TotalProteinas,
            TotalCarbohidratos = original.TotalCarbohidratos,
            TotalGrasas = original.TotalGrasas,
            Semaforo = original.Semaforo,
            EsSaludable = original.EsSaludable,
            DetalleAlimentosResumen = original.DetalleAlimentosResumen,
            EstudianteId = original.EstudianteId,
            NutricionistaId = original.NutricionistaId,
            FechaCreacion = DateTime.UtcNow
        };

        foreach (var item in original.Items)
        {
            clon.Items.Add(new DetalleMenuAlimento
            {
                AlimentoId = item.AlimentoId,
                Cantidad = item.Cantidad,
                Unidad = item.Unidad,
                SubtotalCalorias = item.SubtotalCalorias,
                SubtotalProteinas = item.SubtotalProteinas,
                SubtotalCarbohidratos = item.SubtotalCarbohidratos,
                SubtotalGrasas = item.SubtotalGrasas
            });
        }

        await _context.MenusDiarios.AddAsync(clon);
        await _context.SaveChangesAsync();

        return Ok(new { exito = true, mensaje = "Menú clonado con éxito.", nuevoMenuId = clon.Id });
    }

    [HttpDelete("menus/{id}")]
    [Authorize(AuthenticationSchemes = AuthSchemes, Roles = "Nutricionista,Admin")]
    public async Task<IActionResult> EliminarMenu(int id)
    {
        var menu = await _context.MenusDiarios
            .Include(m => m.Items)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (menu == null)
            return NotFound();

        _context.DetallesMenuAlimentos.RemoveRange(menu.Items);
        _context.MenusDiarios.Remove(menu);
        await _context.SaveChangesAsync();

        return Ok(new { exito = true, mensaje = "Menú eliminado correctamente." });
    }

    // Helpers
    private static string CalcularSemaforo(decimal calorias, decimal grasas)
    {
        if (calorias <= 180 && grasas <= 5) return "Verde";
        if (calorias <= 350 && grasas <= 15) return "Amarillo";
        return "Rojo";
    }

    private static string AsignarIcono(string categoria)
    {
        return categoria.ToLower() switch
        {
            "carnes" => "🥩",
            "frutas" => "🍎",
            "verduras" => "🥦",
            "lácteos" => "🥛",
            "lacteos" => "🥛",
            "cereales" => "🌾",
            "saludables" => "🥗",
            "snacks" => "🥨",
            "bebidas" => "🥤",
            _ => "🥗"
        };
    }
}

public class GuardarMenuDto
{
    public string NombreMenu { get; set; } = string.Empty;
    public DateTime? FechaMenu { get; set; }
    public string? TipoComida { get; set; }
    public string? EstadoVariante { get; set; }
    public decimal TotalCalorias { get; set; }
    public decimal TotalProteinas { get; set; }
    public decimal TotalCarbohidratos { get; set; }
    public decimal TotalGrasas { get; set; }
    public string? Semaforo { get; set; }
    public int? EstudianteId { get; set; }
    public List<GuardarItemMenuDto>? Items { get; set; }
}

public class GuardarItemMenuDto
{
    public int AlimentoId { get; set; }
    public decimal Cantidad { get; set; }
    public string? Unidad { get; set; }
    public decimal SubtotalCalorias { get; set; }
    public decimal SubtotalProteinas { get; set; }
    public decimal SubtotalCarbohidratos { get; set; }
    public decimal SubtotalGrasas { get; set; }
}
