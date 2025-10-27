using System;
using System.Collections.Generic;

using UnityEngine;

public class MachineManager : MonoBehaviour
{
    public static MachineManager Instance { get; private set; }

    private List<RegistroTrabajo> registros = new List<RegistroTrabajo>();
    private RegistroTrabajo registroActual;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 🔹 Inicia un nuevo registro (solo uno activo a la vez)
    public void IniciarNuevoRegistro()
    {
        if (registroActual != null)
        {
            Debug.LogWarning("⚠️ Ya hay un registro en curso. Termina el actual antes de iniciar uno nuevo.");
            return;
        }

        registroActual = new RegistroTrabajo
        {
            id = Guid.NewGuid().ToString(),
            nombreMaquina = "Máquina sin nombre",
            fechaInicio = DateTime.Now.ToString("s"),
            horasTrabajadas = 0f,
            litrosConsumidos = 0f,
            oroGenerado = 0f,
            costoCombustible = 0f
        };

        Debug.Log($"✅ Nuevo registro iniciado a las {registroActual.fechaInicio}");
    }

    // 🔹 Termina el registro actual y lo guarda
    public void TerminarRegistroActual()
    {
        if (registroActual == null)
        {
            Debug.LogWarning("⚠️ No hay ningún registro activo para finalizar.");
            return;
        }

        registroActual.fechaFin = DateTime.Now.ToString("s");
        registros.Add(registroActual);
        Debug.Log($"🧾 Registro finalizado: {registroActual.nombreMaquina} ({registroActual.fechaInicio} → {registroActual.fechaFin})");

        registroActual = null;
    }

    public void RegistrarTrabajo(RegistroTrabajo r)
    {
        if (r == null) return;
        registros.Add(r);
        Debug.Log($"MachineManager: registro agregado {r.nombreMaquina} - {r.horasTrabajadas:F2}h");
    }

    public List<RegistroTrabajo> ObtenerRegistros() => new List<RegistroTrabajo>(registros);

    public void LimpiarRegistros()
    {
        registros.Clear();
        registroActual = null;
        Debug.Log("🧹 Todos los registros fueron eliminados.");
    }
}
