using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartNutriTracker.Web.Modules.Notificaciones.Models;

namespace SmartNutriTracker.Web.Modules.Notificaciones;

public class IndexitoModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexitoModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public List<NotificacionItem> Notificaciones { get; set; } = new();
    public string FiltroActual { get; set; } = "Todas";

    [BindProperty] public string NuevoTitulo { get; set; } = string.Empty;
    [BindProperty] public string NuevoMensaje { get; set; } = string.Empty;
    [BindProperty] public string NuevoTipo { get; set; } = "Recordatorio";
    [BindProperty] public string NuevaSeveridad { get; set; } = "Info";

    public void OnGet(string? filtro)
    {
        FiltroActual = string.IsNullOrWhiteSpace(filtro) ? "Todas" : filtro;
        CargarLista();
    }

    public IActionResult OnPostMarcarLeida(int id, string? filtro)
    {
        var item = NotificacionesMockStore.Items.FirstOrDefault(n => n.Id == id);
        if (item is not null)
        {
            item.Leida = true;
            _logger.LogInformation("[AUDITORÍA NOTIFICACIONES] Notificación #{Id} marcada como leída.", id);
        }
        return RedirectToPage(new { filtro });
    }

    public IActionResult OnPostCrear()
    {
        if (!string.IsNullOrWhiteSpace(NuevoTitulo) && !string.IsNullOrWhiteSpace(NuevoMensaje))
        {
            int nextId = NotificacionesMockStore.Items.Any() ? NotificacionesMockStore.Items.Max(x => x.Id) + 1 : 1;
            NotificacionesMockStore.Items.Insert(0, new NotificacionItem
            {
                Id = nextId,
                Tipo = NuevoTipo,
                Severidad = NuevoTipo == "Alerta" ? NuevaSeveridad : "Info",
                Destinatario = NuevoTipo == "Alerta" ? "Docente" : "Estudiante",
                Titulo = NuevoTitulo,
                Mensaje = NuevoMensaje,
                Fecha = DateTime.Now,
                Leida = false
            });

            _logger.LogInformation("[AUDITORÍA NOTIFICACIONES] Nueva {Tipo} creada: {Titulo}", NuevoTipo, NuevoTitulo);
        }
        return RedirectToPage();
    }

    private void CargarLista()
    {
        var query = NotificacionesMockStore.Items.AsEnumerable();
        if (FiltroActual == "Alerta" || FiltroActual == "Recordatorio")
        {
            query = query.Where(n => n.Tipo == FiltroActual);
        }
        Notificaciones = query.OrderBy(n => n.Leida).ThenByDescending(n => n.Fecha).ToList();
    }
}