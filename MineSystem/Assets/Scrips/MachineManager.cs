using UnityEngine;
using System.Collections.Generic;


public class MachineManager : MonoBehaviour
{
    public static MachineManager Instance;

    private List<RegistroTrabajo> registros = new List<RegistroTrabajo>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 🔹 Método actualizado: recibe directamente un objeto RegistroTrabajo
    public void RegistrarTrabajo(RegistroTrabajo nuevoRegistro)
    {
        if (nuevoRegistro == null)
        {
            Debug.LogWarning("⚠️ Intento de registrar un trabajo nulo.");
            return;
        }

        registros.Add(nuevoRegistro);
        Debug.Log($"🧾 Registro guardado: {nuevoRegistro.nombreMaquina} - {nuevoRegistro.horasTrabajadas}h - {nuevoRegistro.oroGenerado} oro");
    }

    // 🔹 Devuelve todos los registros (por si los necesitas exportar)
    public List<RegistroTrabajo> GetRegistros()
    {
        return registros;
    }

    // 🔹 Limpia la lista si se necesita reiniciar todo
    public void LimpiarRegistros()
    {
        registros.Clear();
        Debug.Log("🧹 Todos los registros fueron eliminados.");
    }


    public List<RegistroTrabajo> ObtenerRegistros()
    {
        return registros; // donde “registros” es la lista interna que ya usas para guardar trabajos
    }
}
