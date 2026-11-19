namespace SmartNutriTracker.Web.Modules.Recomendaciones.Models;

public class EstudianteSimulado
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int Edad { get; set; }
    public double PesoKg { get; set; }
    public double AlturaCm { get; set; }
    public string Objetivo { get; set; } = "Mantener"; // Bajar, Mantener, Ganar
}

public class ResultadoRecomendacion
{
    public double Imc { get; set; }
    public string CategoriaImc { get; set; } = string.Empty;
    public string ColorBadge { get; set; } = "bg-emerald-100 text-emerald-800";
    public int CaloriasDiarias { get; set; }
    public int ProteinasG { get; set; }
    public int CarbohidratosG { get; set; }
    public int GrasasG { get; set; }
    public string Consejo { get; set; } = string.Empty;
    public List<PlatoRecomendado> MenuSugerido { get; set; } = new();
}

public class PlatoRecomendado
{
    public string TiempoComida { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Calorias { get; set; }
    public string Detalle { get; set; } = string.Empty;
}

public static class MotorRecomendacionesMock
{
    public static List<EstudianteSimulado> Estudiantes { get; } = new()
    {
        new() { Id = 1, Nombre = "Lucía Mendoza", Edad = 15, PesoKg = 48, AlturaCm = 162, Objetivo = "Ganar" },
        new() { Id = 2, Nombre = "Mateo Vargas", Edad = 16, PesoKg = 64, AlturaCm = 172, Objetivo = "Mantener" },
        new() { Id = 3, Nombre = "Sofía Rojas", Edad = 14, PesoKg = 74, AlturaCm = 160, Objetivo = "Bajar" }
    };

    public static ResultadoRecomendacion Calcular(double pesoKg, double alturaCm, int edad, string objetivo)
    {
        double alturaM = alturaCm / 100.0;
        double imc = Math.Round(pesoKg / (alturaM * alturaM), 1);

        string categoria;
        string color;
        if (imc < 18.5) { categoria = "Bajo peso"; color = "bg-sky-100 text-sky-800"; }
        else if (imc < 25) { categoria = "Peso normal"; color = "bg-emerald-100 text-emerald-800"; }
        else if (imc < 30) { categoria = "Sobrepeso"; color = "bg-amber-100 text-amber-800"; }
        else { categoria = "Obesidad"; color = "bg-rose-100 text-rose-800"; }

        // Tasa metabólica base simplificada + factor actividad escolar (1.4)
        double baseCalorias = ((10 * pesoKg) + (6.25 * alturaCm) - (5 * edad) + 5) * 1.4;

        int ajuste = objetivo switch
        {
            "Bajar" => -350,
            "Ganar" => 350,
            _ => 0
        };

        int calorias = Math.Max(1300, (int)Math.Round(baseCalorias + ajuste));

        // Distribución de macros: 25% proteínas, 50% carbohidratos, 25% grasas
        int proteinas = (int)Math.Round((calorias * 0.25) / 4);
        int carbos = (int)Math.Round((calorias * 0.50) / 4);
        int grasas = (int)Math.Round((calorias * 0.25) / 9);

        var menu = objetivo switch
        {
            "Bajar" => new List<PlatoRecomendado>
            {
                new() { TiempoComida = "Desayuno", Nombre = "Avena con chía y manzana verde", Calorias = 320, Detalle = "Alto en fibra para saciedad prolongada." },
                new() { TiempoComida = "Almuerzo", Nombre = "Pechuga a la plancha con quinua y ensalada", Calorias = 520, Detalle = "Proteína magra y carbohidrato complejo." },
                new() { TiempoComida = "Merienda", Nombre = "Yogurt griego natural con almendras", Calorias = 180, Detalle = "Snack bajo en azúcares añadidos." },
                new() { TiempoComida = "Cena", Nombre = "Crema de zapallo y tortilla de claras", Calorias = 340, Detalle = "Cena ligera de fácil digestión." }
            },
            "Ganar" => new List<PlatoRecomendado>
            {
                new() { TiempoComida = "Desayuno", Nombre = "Licuado de plátano, avena, leche y maní", Calorias = 540, Detalle = "Alta densidad calórica saludable." },
                new() { TiempoComida = "Almuerzo", Nombre = "Salmón o carne magra con arroz integral y palta", Calorias = 780, Detalle = "Grasas saludables y proteína completa." },
                new() { TiempoComida = "Merienda", Nombre = "Sándwich integral de pollo y queso", Calorias = 420, Detalle = "Aporte extra de energía para la tarde." },
                new() { TiempoComida = "Cena", Nombre = "Pasta integral con atún y brócoli", Calorias = 610, Detalle = "Recuperación muscular nocturna." }
            },
            _ => new List<PlatoRecomendado>
            {
                new() { TiempoComida = "Desayuno", Nombre = "Tostadas integrales con huevo y fruta", Calorias = 410, Detalle = "Energía equilibrada para clases." },
                new() { TiempoComida = "Almuerzo", Nombre = "Pollo al horno con papas doradas y vegetales", Calorias = 650, Detalle = "Plato balanceado 50% vegetales, 25% proteína, 25% carbos." },
                new() { TiempoComida = "Merienda", Nombre = "Fruta de estación con puñado de nueces", Calorias = 230, Detalle = "Micronutrientes y grasas buenas." },
                new() { TiempoComida = "Cena", Nombre = "Ensalada tibia de garbanzos y pollo", Calorias = 460, Detalle = "Proteína vegetal y animal balanceada." }
            }
        };

        string consejo = objetivo switch
        {
            "Bajar" => "Prioriza hidratación (2L de agua), verduras en cada comida y evita bebidas azucaradas.",
            "Ganar" => "Incluye snacks nutritivos entre clases y no te saltes el desayuno.",
            _ => "Mantén horarios regulares de comida y al menos 45 minutos de actividad física diaria."
        };

        return new ResultadoRecomendacion
        {
            Imc = imc,
            CategoriaImc = categoria,
            ColorBadge = color,
            CaloriasDiarias = calorias,
            ProteinasG = proteinas,
            CarbohidratosG = carbos,
            GrasasG = grasas,
            Consejo = consejo,
            MenuSugerido = menu
        };
    }
}