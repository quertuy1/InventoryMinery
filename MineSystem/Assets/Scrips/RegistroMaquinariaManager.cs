using System;
using System.Collections.Generic;
using UnityEngine;

public class RegistroMaquinariaManager : MonoBehaviour
{
    public static RegistroMaquinariaManager Instance;

    [Header("Referencias")]
    public FuelCalculator fuelCalculator; // Asignar desde el inspector

    [Header("Datos de máquina")]
    public string nombreMaquinaActual;
    public float horasTrabajadas;
    public float litrosConsumidos;

    [Header("Registros")]
    public List<RegistroTrabajo> registros = new List<RegistroTrabajo>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Llamar cuando se quiera registrar un trabajo
    public void RegistrarTrabajo()
    {
        if (fuelCalculator == null)
        {
            Debug.LogError("❌ FuelCalculator no asignado en RegistroMaquinariaManager.");
            return;
        }

        RegistroTrabajo nuevoRegistro = new RegistroTrabajo();

        nuevoRegistro.id = Guid.NewGuid().ToString();
        nuevoRegistro.nombreMaquina = nombreMaquinaActual;
        nuevoRegistro.horasTrabajadas = horasTrabajadas;
        nuevoRegistro.litrosConsumidos = litrosConsumidos;

        // 🔹 Costo de combustible
        nuevoRegistro.costoCombustible = fuelCalculator.CalcularCostoPorLitros(litrosConsumidos);

        // 🔹 Oro generado según las horas trabajadas y el precio actual del oro
        if (PriceManager.Instance != null)
        {
            float oroPorHora = PriceManager.Instance.GetGoldPrice();
            nuevoRegistro.oroGenerado = horasTrabajadas * oroPorHora;
        }
        else
        {
            nuevoRegistro.oroGenerado = 0;
        }

        // 🔹 Fechas en formato ISO (compatible con tu clase RegistroTrabajo)
        nuevoRegistro.fechaInicio = DateTime.Now.ToString("o");
        nuevoRegistro.fechaFin = DateTime.Now.ToString("o");

        // 🔹 Agregar registro a la lista
        registros.Add(nuevoRegistro);

        Debug.Log($"✅ Registro creado para {nuevoRegistro.nombreMaquina} con {nuevoRegistro.horasTrabajadas}h y {nuevoRegistro.litrosConsumidos}L.");
    }

    // Obtener todos los registros (por si los necesitas para exportar)
    public List<RegistroTrabajo> ObtenerRegistros()
    {
        return registros;
    }
}
