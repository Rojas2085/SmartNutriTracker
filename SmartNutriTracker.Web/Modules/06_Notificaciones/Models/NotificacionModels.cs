namespace SmartNutriTracker.Web.Modules.Notificaciones.Models;

public class NotificacionItem
{
    public int Id { get; set; }
    public string Tipo { get; set; } = "Recordatorio"; // "Alerta" o "Recordatorio"
    public string Severidad { get; set; } = "Info";    // "Alta", "Media", "Info"
    public string Destinatario { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public bool Leida { get; set; }
}

public static class NotificacionesMockStore
{
    public static List<NotificacionItem> Items { get; } = new()
    {
        new()
        {
            Id = 1, Tipo = "Alerta", Severidad = "Alta", Destinatario = "Docente",
            Titulo = "Exceso calórico detectado",
            Mensaje = "Sofía Rojas superó el límite diario en +520 kcal durante 3 días consecutivos.",
            Fecha = DateTime.Now.AddMinutes(-25), Leida = false
        },
        new()
        {
            Id = 2, Tipo = "Alerta", Severidad = "Media", Destinatario = "Docente",
            Titulo = "Bajo consumo de agua",
            Mensaje = "Lucía Mendoza registró menos de 1L de agua en los últimos 2 días.",
            Fecha = DateTime.Now.AddHours(-2), Leida = false
        },
        new()
        {
            Id = 3, Tipo = "Recordatorio", Severidad = "Info", Destinatario = "Estudiante",
            Titulo = "Registra tu almuerzo",
            Mensaje = "Recuerda cargar los alimentos consumidos hoy antes de las 15:00.",
            Fecha = DateTime.Now.AddHours(-3), Leida = false
        },
        new()
        {
            Id = 4, Tipo = "Recordatorio", Severidad = "Info", Destinatario = "Estudiante",
            Titulo = "Control de horas de sueño",
            Mensaje = "Completa tu registro diario de sueño y actividad física.",
            Fecha = DateTime.Now.AddDays(-1), Leida = true
        }
    };
}