using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class MachineData
{
    public string nombreMaquina;
    public float horasTrabajadas;
    public float oroGenerado;
    public float combustibleActual;

    public MachineData(string nombre, float horas, float oro, float combustible)
    {
        nombreMaquina = nombre;
        horasTrabajadas = horas;
        oroGenerado = oro;
        combustibleActual = combustible;
    }
}

public class MachineStorage : MonoBehaviour
{
    public static MachineStorage Instance;

    [SerializeField] private List<MachineData> maquinasGuardadas = new List<MachineData>();

    private void Awake()
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

    // 🟢 Guardar nueva máquina
    public void GuardarMaquina(MachineData nuevaMaquina)
    {
        if (nuevaMaquina == null)
        {
            Debug.LogWarning("⚠️ Se intentó guardar una máquina nula.");
            return;
        }

        maquinasGuardadas.Add(nuevaMaquina);
        Debug.Log($"💾 Máquina guardada: {nuevaMaquina.nombreMaquina}");
    }

    // 🟡 Obtener todas las máquinas
    public List<MachineData> ObtenerTodas()
    {
        return maquinasGuardadas;
    }

    // 🔵 Obtener una máquina por nombre
    public MachineData ObtenerMaquina(string nombre)
    {
        return maquinasGuardadas.Find(m => m.nombreMaquina == nombre);
    }

    // 🔴 Eliminar una máquina por nombre
    public void EliminarMaquina(string nombre)
    {
        MachineData maquina = maquinasGuardadas.Find(m => m.nombreMaquina == nombre);
        if (maquina != null)
        {
            maquinasGuardadas.Remove(maquina);
            Debug.Log($"🗑️ Máquina eliminada: {nombre}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró la máquina con nombre: {nombre}");
        }
    }

    // 🧹 Limpiar todas las máquinas
    public void LimpiarTodo()
    {
        maquinasGuardadas.Clear();
        Debug.Log("🧹 Todas las máquinas fueron eliminadas del registro.");
    }
}
