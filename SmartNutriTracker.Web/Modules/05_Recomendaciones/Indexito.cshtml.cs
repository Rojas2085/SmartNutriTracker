using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartNutriTracker.Web.Modules.Recomendaciones.Models;

namespace SmartNutriTracker.Web.Modules.Recomendaciones;

public class IndexitoModel : PageModel
{
    public List<EstudianteSimulado> Estudiantes { get; set; } = MotorRecomendacionesMock.Estudiantes;

    [BindProperty] public int EstudianteSeleccionadoId { get; set; } = 1;
    [BindProperty] public double PesoKg { get; set; } = 64;
    [BindProperty] public double AlturaCm { get; set; } = 172;
    [BindProperty] public int Edad { get; set; } = 16;
    [BindProperty] public string Objetivo { get; set; } = "Mantener";

    public ResultadoRecomendacion? Resultado { get; set; }

    public void OnGet(int? estudianteId)
    {
        var est = Estudiantes.FirstOrDefault(e => e.Id == (estudianteId ?? 2)) ?? Estudiantes.First();
        EstudianteSeleccionadoId = est.Id;
        PesoKg = est.PesoKg;
        AlturaCm = est.AlturaCm;
        Edad = est.Edad;
        Objetivo = est.Objetivo;

        Resultado = MotorRecomendacionesMock.Calcular(PesoKg, AlturaCm, Edad, Objetivo);
    }

    public void OnPost()
    {
        Resultado = MotorRecomendacionesMock.Calcular(PesoKg, AlturaCm, Edad, Objetivo);
    }
}