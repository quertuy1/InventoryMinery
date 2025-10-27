using System;

using UnityEngine;

public class MachineTimer : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreMaquina = "Maquina";
    public float litrosPorHora = 5f;
    public float oroPorHora = 1f;
    public float combustibleActual = 100f;

    private bool trabajando = false;
    private DateTime inicioTrabajo;
    private float horasAcumuladas = 0f; // acumulado entre inicios/finales
    private float litrosConsumidosAcumulado = 0f;
    private float oroGeneradoAcumulado = 0f;

    // live tracking while working
    public void IniciarTrabajo()
    {
        if (trabajando) return;
        if (combustibleActual <= 0f) { Debug.LogWarning($"{nombreMaquina}: sin combustible"); return; }
        trabajando = true;
        inicioTrabajo = DateTime.Now;
    }

    public void FinalizarTrabajo()
    {
        if (!trabajando) return;
        DateTime fin = DateTime.Now;
        float horas = (float)(fin - inicioTrabajo).TotalHours;
        if (horas < 0f) horas = 0f;

        float litrosConsumidos = Mathf.Min(combustibleActual, litrosPorHora * horas);
        float oroGenerado = oroPorHora * horas;

        // acumular
        horasAcumuladas += horas;
        litrosConsumidosAcumulado += litrosConsumidos;
        oroGeneradoAcumulado += oroGenerado;
        combustibleActual -= litrosConsumidos;
        if (combustibleActual < 0f) combustibleActual = 0f;

        trabajando = false;
    }

    private void Update()
    {
        // nothing expensive here; keep simple. RegistroUI will request getters.
    }

    // Getters used by UI / Registro
    public float GetHorasTrabajadas()
    {
        if (trabajando) return horasAcumuladas + (float)(DateTime.Now - inicioTrabajo).TotalHours;
        return horasAcumuladas;
    }

    public float GetLitrosConsumidos()
    {
        if (trabajando)
        {
            float horas = (float)(DateTime.Now - inicioTrabajo).TotalHours;
            return litrosConsumidosAcumulado + Mathf.Min(combustibleActual, litrosPorHora * horas);
        }
        return litrosConsumidosAcumulado;
    }

    public float GetOroGenerado()
    {
        if (trabajando)
        {
            float horas = (float)(DateTime.Now - inicioTrabajo).TotalHours;
            return oroGeneradoAcumulado + oroPorHora * horas;
        }
        return oroGeneradoAcumulado;
    }

    public float GetCombustibleActual() => combustibleActual;

    public void SetCombustible(float nuevo) => combustibleActual = Mathf.Max(0f, nuevo);

    // Produce MachineData snapshot (no modifica stored accumulators)
    public MachineData CrearSnapshot()
    {
        return new MachineData(nombreMaquina, litrosPorHora, oroPorHora, combustibleActual);
    }
}
