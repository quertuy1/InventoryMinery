using UnityEngine;
using System;


public class MachineWorkController : MonoBehaviour
{
    [Header("Datos de la máquina")]
    public string nombreMaquina;
    public float litrosConsumidos;
    public float oroGenerado;
    public float horasTrabajadas;
    public float costoCombustible;

    private DateTime fechaInicio;
    private DateTime fechaFin;
    private bool trabajando = false;

    public void IniciarTrabajo()
    {
        if (!trabajando)
        {
            trabajando = true;
            fechaInicio = DateTime.Now;
            Debug.Log($"✅ {nombreMaquina} inició trabajo a las {fechaInicio}");
        }
    }

    public void FinalizarTrabajo()
    {
        if (trabajando)
        {
            trabajando = false;
            fechaFin = DateTime.Now;

            RegistroTrabajo registro = new RegistroTrabajo
            {
                id = Guid.NewGuid().ToString(),
                nombreMaquina = nombreMaquina,
                horasTrabajadas = horasTrabajadas,
                litrosConsumidos = litrosConsumidos,
                oroGenerado = oroGenerado,
                costoCombustible = costoCombustible,
                fechaInicio = fechaInicio.ToString("s"),
                fechaFin = fechaFin.ToString("s")
            };

            MachineManager.Instance.RegistrarTrabajo(registro);

            Debug.Log($"🧾 Registro finalizado para {nombreMaquina}: {horasTrabajadas}h, {litrosConsumidos}L, {oroGenerado} oro");
        }
    }
}
