using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// Muestra la lista de registros guardados en MachineManager y permite exportarlos a JSON.
/// Requiere:
/// - contenedorRegistros: Content del ScrollView
/// - prefabRegistro: prefab con varios TMP_Text (nombres esperados: Nombre, Horas, Litros, Oro, Costo, FechaInicio, FechaFin)
/// - botonExportar: (opcional) botón que dispara ExportarRegistros()
/// - botonInicio y botonFin: botones para iniciar y terminar el registro manualmente
/// </summary>
public class RegistroUI : MonoBehaviour
{
    [Header("UI")]
    public Transform contenedorRegistros;
    public GameObject prefabRegistro;
    public Button botonExportar;
    public Button botonInicio;
    public Button botonFin;

    private MachineManager manager;

    private void Start()
    {
        manager = MachineManager.Instance;

        if (manager == null)
        {
            Debug.LogError("RegistroUI: MachineManager.Instance es null.");
            return;
        }

        if (botonExportar != null)
            botonExportar.onClick.AddListener(ExportarRegistros);

        if (botonInicio != null)
            botonInicio.onClick.AddListener(IniciarRegistro);

        if (botonFin != null)
            botonFin.onClick.AddListener(TerminarRegistro);

        ActualizarLista();
    }

    public void ActualizarLista()
    {
        if (contenedorRegistros == null || prefabRegistro == null)
        {
            Debug.LogWarning("RegistroUI: contenedorRegistros o prefabRegistro no asignado.");
            return;
        }

        // limpiar registros anteriores
        for (int i = contenedorRegistros.childCount - 1; i >= 0; i--)
            Destroy(contenedorRegistros.GetChild(i).gameObject);

        List<RegistroTrabajo> regs = manager.ObtenerRegistros();
        if (regs == null || regs.Count == 0)
        {
            Debug.Log("RegistroUI: no hay registros para mostrar.");
            return;
        }

        foreach (var r in regs)
        {
            GameObject go = Instantiate(prefabRegistro, contenedorRegistros);
            TMP_Text[] textos = go.GetComponentsInChildren<TMP_Text>(true);

            if (textos.Length == 0)
            {
                Debug.LogWarning($"RegistroUI: el prefab {prefabRegistro.name} no contiene TMP_Text.");
                continue;
            }

            // asignación por nombre del objeto
            foreach (var t in textos)
            {
                switch (t.name)
                {
                    case "Nombre": t.text = r.nombreMaquina; break;
                    case "Horas": t.text = $"{r.horasTrabajadas:F2} h"; break;
                    case "Litros": t.text = $"{r.litrosConsumidos:F2} L"; break;
                    case "Oro": t.text = $"{r.oroGenerado:F2} oro"; break;
                    case "Costo": t.text = $"${r.costoCombustible:F2}"; break;
                    case "FechaInicio": t.text = r.fechaInicio; break;
                    case "FechaFin": t.text = r.fechaFin; break;
                }
            }
        }
    }

    public void ExportarRegistros()
    {
        var regs = manager.ObtenerRegistros();
        if (regs == null || regs.Count == 0)
        {
            Debug.Log("RegistroUI: no hay registros para exportar.");
            return;
        }

        try
        {
            string folder = Application.persistentDataPath;
            string fileName = $"registros_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            string path = Path.Combine(folder, fileName);

            var wrapper = new Wrapper { lista = regs };
            string json = JsonUtility.ToJson(wrapper, true);

            File.WriteAllText(path, json);
            Debug.Log($"✅ Registros exportados correctamente a: {path}");
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error exportando registros: " + ex.Message);
        }
    }

    private void IniciarRegistro()
    {
        manager.IniciarNuevoRegistro();
        ActualizarLista();
    }

    private void TerminarRegistro()
    {
        manager.TerminarRegistroActual();
        ActualizarLista();
    }

    [Serializable]
    private class Wrapper { public List<RegistroTrabajo> lista; }
}
