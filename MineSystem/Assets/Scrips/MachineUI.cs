using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MachineUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TMP_InputField nombreInput;
    [SerializeField] private TMP_InputField horasInput;
    [SerializeField] private TMP_InputField oroInput;
    [SerializeField] private TMP_InputField combustibleInput;
    [SerializeField] private TMP_Dropdown listaMaquinasDropdown;

    [Header("Botones")]
    [SerializeField] private Button btnGuardar;
    [SerializeField] private Button btnMostrar;
    [SerializeField] private Button btnEliminar;

    [Header("Texto de información")]
    [SerializeField] private TMP_Text detallesTexto;

    private MachineStorage machineStorage;

    private void Start()
    {
        machineStorage = MachineStorage.Instance;

        if (btnGuardar != null)
            btnGuardar.onClick.AddListener(GuardarMaquina);

        if (btnMostrar != null)
            btnMostrar.onClick.AddListener(MostrarDetalles);

        if (btnEliminar != null)
            btnEliminar.onClick.AddListener(EliminarMaquina);

        ActualizarListaDropdown();
    }

    private void GuardarMaquina()
    {
        string nombre = nombreInput.text;
        if (string.IsNullOrEmpty(nombre))
        {
            detallesTexto.text = "⚠️ Ingresa un nombre de máquina.";
            return;
        }

        float horas = float.TryParse(horasInput.text, out float h) ? h : 0f;
        float oro = float.TryParse(oroInput.text, out float o) ? o : 0f;
        float combustible = float.TryParse(combustibleInput.text, out float c) ? c : 0f;

        MachineData nueva = new MachineData(nombre, horas, oro, combustible);
        machineStorage.GuardarMaquina(nueva);

        detallesTexto.text = $"✅ Máquina '{nombre}' guardada correctamente.";
        ActualizarListaDropdown();
    }

    private void MostrarDetalles()
    {
        if (listaMaquinasDropdown.options.Count == 0)
        {
            detallesTexto.text = "⚠️ No hay máquinas guardadas.";
            return;
        }

        string seleccion = listaMaquinasDropdown.options[listaMaquinasDropdown.value].text;
        MachineData maquina = machineStorage.ObtenerMaquina(seleccion);

        if (maquina != null)
        {
            detallesTexto.text =
                $"🧭 Nombre: {maquina.nombreMaquina}\n" +
                $"⏱ Horas: {maquina.horasTrabajadas}\n" +
                $"💰 Oro: {maquina.oroGenerado}\n" +
                $"⛽ Combustible: {maquina.combustibleActual}";
        }
        else
        {
            detallesTexto.text = $"⚠️ No se encontró la máquina '{seleccion}'.";
        }
    }

    private void EliminarMaquina()
    {
        if (listaMaquinasDropdown.options.Count == 0)
        {
            detallesTexto.text = "⚠️ No hay máquinas para eliminar.";
            return;
        }

        string seleccion = listaMaquinasDropdown.options[listaMaquinasDropdown.value].text;
        machineStorage.EliminarMaquina(seleccion);

        detallesTexto.text = $"🗑️ Máquina '{seleccion}' eliminada.";
        ActualizarListaDropdown();
    }

    private void ActualizarListaDropdown()
    {
        listaMaquinasDropdown.ClearOptions();

        List<MachineData> maquinas = machineStorage.ObtenerTodas();
        List<string> nombres = new List<string>();

        foreach (var maquina in maquinas)
        {
            nombres.Add(maquina.nombreMaquina);
        }

        listaMaquinasDropdown.AddOptions(nombres);
    }
    public void update()
    {
        ActualizarListaDropdown();
    }
}

