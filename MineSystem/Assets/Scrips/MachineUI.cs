using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class MachineUI : MonoBehaviour
{
    [Header("Crear máquina")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputLitrosPorHora;
    public TMP_InputField inputOroPorHora;
    public TMP_InputField inputCombustibleInicial;
    public Button botonCrear;

    [Header("Gestión")]
    public TMP_Dropdown dropdownMaquinas;
    public TMP_InputField inputNuevoCombustible;
    public Button botonActualizarCombustible;
    public Button botonEliminar;
    public TextMeshProUGUI textoFeedback;

    private void Start()
    {
        botonCrear.onClick.AddListener(OnCrearMaquina);
        botonActualizarCombustible.onClick.AddListener(OnActualizarCombustible);
        botonEliminar.onClick.AddListener(OnEliminarMaquina);
        ActualizarDropdown();
    }

    public void ActualizarDropdown()
    {
        dropdownMaquinas.ClearOptions();
        var list = MachineStorage.Instance != null ? MachineStorage.Instance.ObtenerMaquinasGuardadas() : new List<MachineData>();
        List<string> names = new List<string>();
        foreach (var m in list) names.Add(m.nombreMaquina);
        if (names.Count == 0) names.Add("(sin máquinas)");
        dropdownMaquinas.AddOptions(names);
        MostrarDetallesSeleccionada();
    }

    public void OnCrearMaquina()
    {
        if (MachineStorage.Instance == null) { Mostrar("No hay MachineStorage"); return; }
        string nombre = inputNombre.text.Trim();
        if (string.IsNullOrEmpty(nombre)) { Mostrar("Nombre inválido"); return; }
        if (!float.TryParse(inputLitrosPorHora.text, out float litros)) { Mostrar("Litros inválido"); return; }
        if (!float.TryParse(inputOroPorHora.text, out float oro)) { Mostrar("Oro inválido"); return; }
        if (!float.TryParse(inputCombustibleInicial.text, out float comb)) comb = 100f;

        var created = MachineStorage.Instance.CrearNuevaMaquina(nombre, litros, oro, comb);
        if (created != null) { Mostrar($"Máquina '{nombre}' creada"); ActualizarDropdown(); }
        else Mostrar($"No se creó. Nombre ya existe?");
    }

    public void OnActualizarCombustible()
    {
        if (MachineStorage.Instance == null) { Mostrar("No hay MachineStorage"); return; }
        if (dropdownMaquinas.options.Count == 0) { Mostrar("No hay máquinas"); return; }
        string nombre = dropdownMaquinas.options[dropdownMaquinas.value].text;
        if (!float.TryParse(inputNuevoCombustible.text, out float nuevo)) { Mostrar("Combustible inválido"); return; }
        bool ok = MachineStorage.Instance.ActualizarCombustible(nombre, nuevo);
        if (ok) { Mostrar($"Combustible de '{nombre}' actualizado"); ActualizarDropdown(); }
        else Mostrar("Error actualizando combustible");
    }

    public void OnEliminarMaquina()
    {
        if (MachineStorage.Instance == null) { Mostrar("No hay MachineStorage"); return; }
        if (dropdownMaquinas.options.Count == 0) { Mostrar("No hay máquinas"); return; }
        string nombre = dropdownMaquinas.options[dropdownMaquinas.value].text;
        bool ok = MachineStorage.Instance.EliminarMaquina(nombre);
        if (ok) { Mostrar($"Máquina '{nombre}' eliminada"); ActualizarDropdown(); }
        else Mostrar("Error al eliminar");
    }

    private void Mostrar(string msg)
    {
        if (textoFeedback != null) textoFeedback.text = msg;
        Debug.Log(msg);
    }

    public void MostrarDetallesSeleccionada()
    {
        // optional: implement to show details in UI; left simple
    }
}
