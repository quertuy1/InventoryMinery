using System;

using UnityEngine;

public class MachineWorkController : MonoBehaviour
{
    public MachineTimer timer; // asignar en inspector (si el componente está en el mismo GO, arrástralo)

    private void Reset()
    {
        if (timer == null) timer = GetComponent<MachineTimer>();
    }

    public void BotonIniciar()
    {
        if (timer == null) { Debug.LogError("MachineWorkController: timer no asignado"); return; }
        timer.IniciarTrabajo();
    }

    public void BotonFinalizar()
    {
        if (timer == null) { Debug.LogError("MachineWorkController: timer no asignado"); return; }

        // take snapshot before finalizing
        float horas = timer.GetHorasTrabajadas();
        float litros = timer.GetLitrosConsumidos();
        float oro = timer.GetOroGenerado();
        // finalize the timer
        timer.FinalizarTrabajo();

        // compute fuel cost via FuelCalculator
        float costo = 0f;
        var fc = FindObjectOfType<FuelCalculator>();
        if (fc != null) costo = fc.CalcularCostoPorLitros(litros);

        RegistroTrabajo reg = new RegistroTrabajo
        {
            id = Guid.NewGuid().ToString(),
            nombreMaquina = timer.nombreMaquina,
            horasTrabajadas = horas,
            litrosConsumidos = litros,
            oroGenerado = oro,
            costoCombustible = costo,
            fechaInicio = DateTime.Now.ToString("s"),
            fechaFin = DateTime.Now.ToString("s")
        };

        MachineManager.Instance.RegistrarTrabajo(reg);
        // persist machine storage (fuel changed)
        if (MachineStorage.Instance != null) MachineStorage.Instance.GuardarMaquinas();
    }
}
