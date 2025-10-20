
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;


public class RegistroExported : MonoBehaviour
{
    // Exporta a TXT (legible)
    public void ExportarRegistrosTxt()
    {
        var regs = MachineManager.Instance?.GetRegistros();
        if (regs == null || regs.Count == 0)
        {
            Debug.LogWarning("No hay registros para exportar (TXT).");
            return;
        }

        string path = Path.Combine(Application.persistentDataPath, "registros_maquinas.txt");
        var sb = new StringBuilder();
        sb.AppendLine("Registros de Maquinaria");
        sb.AppendLine("Export: " + System.DateTime.Now.ToString("g"));
        sb.AppendLine("----------------------------------------");

        foreach (var r in regs)
        {
            sb.AppendLine($"ID: {r.id}");
            sb.AppendLine($"Máquina: {r.nombreMaquina}");
            sb.AppendLine($"Horas: {r.horasTrabajadas:F2}");
            sb.AppendLine($"Litros: {r.litrosConsumidos:F2}");
            sb.AppendLine($"Oro generado: {r.oroGenerado:F2}");
            sb.AppendLine($"Costo combustible: {r.costoCombustible:F2}");
            sb.AppendLine($"Inicio: {r.fechaInicio}");
            sb.AppendLine($"Fin: {r.fechaFin}");
            sb.AppendLine("----------------------------------------");
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        Debug.Log($"Exportado TXT: {path}");
    }

    // Exporta a CSV (útil para Excel)
    public void ExportarRegistrosCsv()
    {
        var regs = MachineManager.Instance?.GetRegistros();
        if (regs == null || regs.Count == 0)
        {
            Debug.LogWarning("No hay registros para exportar (CSV).");
            return;
        }

        string path = Path.Combine(Application.persistentDataPath, "registros_maquinas.csv");
        var sb = new StringBuilder();
        sb.AppendLine("id,nombre,horas,litros,oro,costo_combustible,fechaInicio,fechaFin");

        foreach (var r in regs)
        {
            // escape básico de comas
            string name = (r.nombreMaquina ?? "").Replace(",", ";");
            sb.AppendLine(string.Join(",",
                EscapeCsv(r.id),
                EscapeCsv(name),
                r.horasTrabajadas.ToString(CultureInfo.InvariantCulture),
                r.litrosConsumidos.ToString(CultureInfo.InvariantCulture),
                r.oroGenerado.ToString(CultureInfo.InvariantCulture),
                r.costoCombustible.ToString(CultureInfo.InvariantCulture),
                $"\"{r.fechaInicio}\"",
                $"\"{r.fechaFin}\""
            ));
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        Debug.Log($"Exportado CSV: {path}");
    }

    private string EscapeCsv(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return "\"" + s.Replace("\"", "\"\"") + "\"";
    }
}
