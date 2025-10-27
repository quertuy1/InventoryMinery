using System;
using System.Collections.Generic;

using System.IO;
using UnityEngine;


public class MachineStorage : MonoBehaviour
{
    public static MachineStorage Instance { get; private set; }

    [Tooltip("Nombre del archivo JSON dentro de Application.persistentDataPath")]
    public string fileName = "maquinas.json";

    [Tooltip("Opcional: padre donde instanciar las máquinas en la jerarquía")]
    public Transform maquinasParent;

    private string savePath;
    private List<MachineData> maquinasGuardadas = new List<MachineData>();
    private List<MachineTimer> maquinasActivas = new List<MachineTimer>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        savePath = Path.Combine(Application.persistentDataPath, fileName);
    }

    private void Start()
    {
        CargarMaquinas();
    }

    // Create / register a new machine (returns the instantiated MachineTimer)
    public MachineTimer CrearNuevaMaquina(string nombre, float litrosPorHora, float oroPorHora, float combustibleInicial = 100f)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return null;
        if (maquinasGuardadas.Exists(m => m.nombreMaquina.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
            return null;

        // Save metadata
        var data = new MachineData(nombre, litrosPorHora, oroPorHora, combustibleInicial);
        maquinasGuardadas.Add(data);

        // Instantiate in scene
        var timer = InstanciarMaquinaDesdeData(data);
        GuardarMaquinas();
        return timer;
    }

    private MachineTimer InstanciarMaquinaDesdeData(MachineData data)
    {
        GameObject go = new GameObject(data.nombreMaquina);
        if (maquinasParent != null) go.transform.SetParent(maquinasParent, false);
        MachineTimer t = go.AddComponent<MachineTimer>();
        t.nombreMaquina = data.nombreMaquina;
        t.litrosPorHora = data.litrosPorHora;
        t.oroPorHora = data.oroPorHora;
        t.combustibleActual = data.combustibleActual;
        maquinasActivas.Add(t);
        return t;
    }

    // Update saved list and persist current active machines' fuel (calls when needed)
    public void GuardarMaquinas()
    {
        // sync maquinasGuardadas with maquinasActivas by name
        maquinasGuardadas.Clear();
        foreach (var m in maquinasActivas)
        {
            if (m == null) continue;
            maquinasGuardadas.Add(new MachineData(m.nombreMaquina, m.litrosPorHora, m.oroPorHora, m.combustibleActual));
        }

        try
        {
            var wrapper = new MachineList { maquinas = maquinasGuardadas };
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(savePath, json);
            Debug.Log($"MachineStorage: guardadas {maquinasGuardadas.Count} máquinas en {savePath}");
        }
        catch (Exception e)
        {
            Debug.LogError("MachineStorage: error guardando máquinas: " + e.Message);
        }
    }

    public void CargarMaquinas()
    {
        maquinasActivas.Clear();
        maquinasGuardadas.Clear();

        if (!File.Exists(savePath)) return;

        try
        {
            string json = File.ReadAllText(savePath);
            var wrapper = JsonUtility.FromJson<MachineList>(json);
            maquinasGuardadas = wrapper?.maquinas ?? new List<MachineData>();
            foreach (var d in maquinasGuardadas)
                InstanciarMaquinaDesdeData(d);
            Debug.Log($"MachineStorage: cargadas {maquinasActivas.Count} máquinas.");
        }
        catch (Exception e)
        {
            Debug.LogError("MachineStorage: error cargando máquinas: " + e.Message);
        }
    }

    // Public getters / operations
    public List<MachineData> ObtenerMaquinasGuardadas() => new List<MachineData>(maquinasGuardadas);
    public List<MachineTimer> ObtenerMaquinasActivas() => new List<MachineTimer>(maquinasActivas);

    public MachineTimer ObtenerMaquinaActivaPorNombre(string nombre)
    {
        return maquinasActivas.Find(m => m != null && m.nombreMaquina.Equals(nombre, StringComparison.OrdinalIgnoreCase));
    }

    public MachineData ObtenerMaquinaGuardada(string nombre)
    {
        return maquinasGuardadas.Find(m => m.nombreMaquina.Equals(nombre, StringComparison.OrdinalIgnoreCase));
    }

    public bool ActualizarCombustible(string nombre, float nuevoCombustible)
    {
        var m = ObtenerMaquinaActivaPorNombre(nombre);
        if (m == null) return false;
        m.SetCombustible(nuevoCombustible);
        GuardarMaquinas();
        return true;
    }

    public bool EliminarMaquina(string nombre)
    {
        // remove saved data
        int removed = maquinasGuardadas.RemoveAll(m => m.nombreMaquina.Equals(nombre, StringComparison.OrdinalIgnoreCase));
        // remove active instance
        var inst = ObtenerMaquinaActivaPorNombre(nombre);
        if (inst != null)
        {
            maquinasActivas.Remove(inst);
            Destroy(inst.gameObject);
        }
        GuardarMaquinas();
        return removed > 0 || inst != null;
    }

    [Serializable]
    private class MachineList { public List<MachineData> maquinas; }
}
