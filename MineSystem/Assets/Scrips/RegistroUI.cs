using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;


public class RegistroUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Transform contenedorRegistros;
    public GameObject prefabRegistro;
    public Button botonRegistrar;
    public Button botonActualizar;
    public TMP_Dropdown dropdownMaquinas; // 👈 Aquí eliges la máquina a registrar

    private FuelCalculator fuelCalc;
    private MachineManager machineManager;
    private List<MachineTimer> maquinasDisponibles = new List<MachineTimer>();

    private void Start()
    {
        fuelCalc = FindObjectOfType<FuelCalculator>();
        machineManager = MachineManager.Instance;

        ActualizarListaMaquinas();

        if (botonRegistrar != null)
            botonRegistrar.onClick.AddListener(RegistrarDesdeSeleccion);

        if (botonActualizar != null)
            botonActualizar.onClick.AddListener(ActualizarLista);
    }

    // 🔄 Carga todas las máquinas activas en el Dropdown
    private void ActualizarListaMaquinas()
    {
        maquinasDisponibles.Clear();
        dropdownMaquinas.ClearOptions();

        MachineTimer[] maquinas = FindObjectsOfType<MachineTimer>();
        foreach (var maquina in maquinas)
        {
            maquinasDisponibles.Add(maquina);
        }

        List<string> nombres = new List<string>();
        foreach (var maquina in maquinasDisponibles)
        {
            nombres.Add(maquina.nombreMaquina);
        }

        if (nombres.Count == 0)
        {
            nombres.Add("No hay máquinas activas");
        }

        dropdownMaquinas.AddOptions(nombres);
    }

    // 🧾 Registra los datos de la máquina seleccionada
    public void RegistrarDesdeSeleccion()
    {
        if (maquinasDisponibles.Count == 0)
        {
            Debug.LogWarning("⚠️ No hay máquinas disponibles para registrar.");
            return;
        }

        if (fuelCalc == null || machineManager == null)
        {
            Debug.LogWarning("⚠️ Faltan referencias en RegistroUI.");
            return;
        }

        int indice = dropdownMaquinas.value;
        if (indice < 0 || indice >= maquinasDisponibles.Count)
        {
            Debug.LogWarning("⚠️ Selección inválida en el menú desplegable.");
            return;
        }

        MachineTimer maquina = maquinasDisponibles[indice];
        if (maquina == null)
        {
            Debug.LogWarning("⚠️ Máquina seleccionada no válida.");
            return;
        }

        float horas = maquina.GetHorasTrabajadas();
        float litrosConsumidos = maquina.GetLitrosConsumidos();
        float costoCombustible = fuelCalc.CalcularCostoPorLitros(litrosConsumidos);
        float oroGenerado = maquina.GetOroGenerado();

        RegistroTrabajo registro = new RegistroTrabajo
        {
            id = Guid.NewGuid().ToString(),
            nombreMaquina = maquina.nombreMaquina,
            horasTrabajadas = horas,
            litrosConsumidos = litrosConsumidos,
            oroGenerado = oroGenerado,
            costoCombustible = costoCombustible,
            fechaInicio = DateTime.Now.ToString("s"),
            fechaFin = DateTime.Now.ToString("s")
        };

        machineManager.RegistrarTrabajo(registro);

        Debug.Log($"🧾 Registro para {maquina.nombreMaquina}: {horas:F2}h | {litrosConsumidos:F2}L | {oroGenerado:F2} oro | ${costoCombustible:F2}");

        ActualizarLista();
    }

    // 🔁 Actualiza visualmente la lista de registros existentes
    public void ActualizarLista()
    {
        if (machineManager == null) return;

        foreach (Transform child in contenedorRegistros)
            Destroy(child.gameObject);

        List<RegistroTrabajo> registros = machineManager.ObtenerRegistros();

        foreach (var registro in registros)
        {
            GameObject nuevoItem = Instantiate(prefabRegistro, contenedorRegistros);
            TMP_Text[] textos = nuevoItem.GetComponentsInChildren<TMP_Text>();

            if (textos.Length >= 7)
            {
                textos[0].text = registro.nombreMaquina;
                textos[1].text = $"{registro.horasTrabajadas:F2} h";
                textos[2].text = $"{registro.litrosConsumidos:F2} L";
                textos[3].text = $"{registro.oroGenerado:F2} oro";
                textos[4].text = $"${registro.costoCombustible:F2}";
                textos[5].text = $"Inicio: {registro.fechaInicio}";
                textos[6].text = $"Fin: {registro.fechaFin}";
            }
        }
    }
}
