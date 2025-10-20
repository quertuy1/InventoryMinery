using UnityEngine;
using System;


public class MachineTimer : MonoBehaviour
{
    [Header("Configuración de Máquina")]
    public string nombreMaquina;
    public float litrosPorHora = 5f; // consumo de combustible por hora
    public float oroPorHora = 2f;    // oro generado por hora
    public float combustibleActual = 100f; // cantidad actual de combustible

    private bool trabajando = false;
    private DateTime inicioTrabajo;
    private float horasTrabajadas = 0f;
    private float litrosConsumidos = 0f;
    private float oroGenerado = 0f;

    void Update()
    {
        if (trabajando)
        {
            // Calcula las horas en tiempo real
            horasTrabajadas = (float)(DateTime.Now - inicioTrabajo).TotalHours;

            // Calcula consumo y producción
            float consumo = litrosPorHora * horasTrabajadas;
            if (consumo > combustibleActual)
            {
                // Si no alcanza el combustible, detiene el trabajo
                FinalizarTrabajo();
            }
        }
    }

    public void IniciarTrabajo()
    {
        if (trabajando) return;
        if (combustibleActual <= 0f)
        {
            Debug.LogWarning($"⚠️ {nombreMaquina} no tiene combustible para iniciar el trabajo.");
            return;
        }

        trabajando = true;
        inicioTrabajo = DateTime.Now;
        Debug.Log($"✅ Máquina '{nombreMaquina}' inició trabajo a las {inicioTrabajo}");
    }

    public void FinalizarTrabajo()
    {
        if (!trabajando) return;
        trabajando = false;

        DateTime finTrabajo = DateTime.Now;
        float tiempoTotal = (float)(finTrabajo - inicioTrabajo).TotalHours;

        // Cálculos principales
        float consumo = litrosPorHora * tiempoTotal;
        if (consumo > combustibleActual)
            consumo = combustibleActual;

        litrosConsumidos += consumo;
        oroGenerado += oroPorHora * tiempoTotal;
        combustibleActual -= consumo;
        horasTrabajadas += tiempoTotal;

        // Calcular costo usando FuelCalculator
        FuelCalculator fuelCalc = FindObjectOfType<FuelCalculator>();
        float costoCombustible = 0f;
        if (fuelCalc != null)
            costoCombustible = fuelCalc.CalcularCostoPorLitros(consumo);

        // Crear registro de trabajo
        RegistroTrabajo registro = new RegistroTrabajo
        {
            id = Guid.NewGuid().ToString(),
            nombreMaquina = nombreMaquina,
            horasTrabajadas = horasTrabajadas,
            litrosConsumidos = litrosConsumidos,
            oroGenerado = oroGenerado,
            costoCombustible = costoCombustible,
            fechaInicio = inicioTrabajo.ToString("yyyy-MM-dd HH:mm:ss"),
            fechaFin = finTrabajo.ToString("yyyy-MM-dd HH:mm:ss")
        };

        MachineManager.Instance.RegistrarTrabajo(registro);

        Debug.Log($"🛑 {nombreMaquina} finalizó trabajo.\n" +
                  $"Tiempo: {horasTrabajadas:F2}h | Litros: {litrosConsumidos:F2}L | " +
                  $"Oro: {oroGenerado:F2} | Costo: ${costoCombustible:F2} | Combustible restante: {combustibleActual:F2}L");

        horasTrabajadas = 0f;
    }

    // 🔹 Métodos públicos para acceder desde RegistroUI o UI de gestión

    public float GetHorasTrabajadas() => horasTrabajadas;
    public float GetLitrosConsumidos() => litrosConsumidos;
    public float GetOroGenerado() => oroGenerado;
    public float GetCombustibleActual() => combustibleActual;

    public void SetCombustible(float nuevoCombustible)
    {
        combustibleActual = Mathf.Max(0, nuevoCombustible);
        Debug.Log($"⛽ Combustible de {nombreMaquina} actualizado a {combustibleActual:F2}L");
    }

    public void ReiniciarDatos()
    {
        horasTrabajadas = 0f;
        litrosConsumidos = 0f;
        oroGenerado = 0f;
    }
}
