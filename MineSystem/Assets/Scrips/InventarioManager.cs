using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;

[System.Serializable]
public class ItemInventario
{
    public string nombre;
    public int cantidad;
    public string categoria; // HERRAMIENTA o COMIDA
    public string id;
}

[System.Serializable]
public class DatabaseInventario
{
    public List<ItemInventario> items = new List<ItemInventario>();
}

public class InventarioManager : MonoBehaviour
{
    // 🔥 SINGLETON GLOBAL — accesible desde cualquier script
    public static InventarioManager Instance;

    [Header("UI Referencias")]
    public Transform contentLista;          // El Content del ScrollView
    public GameObject itemPrefab;           // Prefab para mostrar cada item

    [Header("Inputs")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputCantidad;
    public TMP_Dropdown dropdownCategoria;

    private string inventarioPath;
    private DatabaseInventario inventarioDB = new DatabaseInventario();

    // =====================================================
    void Awake()
    {
        // --- Configurar Singleton persistente ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inventarioPath = Path.Combine(Application.persistentDataPath, "inventario_mina.json");
        CargarInventario();
    }

    // =====================================================
    void Start()
    {
        MostrarInventario();
    }

    // =====================================================
    void CargarInventario()
    {
        if (!File.Exists(inventarioPath))
        {
            inventarioDB = new DatabaseInventario();
            GuardarInventario();
            return;
        }

        string json = File.ReadAllText(inventarioPath);
        inventarioDB = JsonUtility.FromJson<DatabaseInventario>(json);
        if (inventarioDB == null || inventarioDB.items == null)
            inventarioDB = new DatabaseInventario();
    }

    void GuardarInventario()
    {
        string json = JsonUtility.ToJson(inventarioDB, true);
        File.WriteAllText(inventarioPath, json);
    }

    // =====================================================
    public void AgregarItem()
    {
        string nombre = inputNombre != null ? inputNombre.text.Trim() : "";
        if (string.IsNullOrEmpty(nombre)) return;

        var existente = inventarioDB.items.Find(i => i.nombre.ToLower() == nombre.ToLower());
        if (existente != null)
        {
            Debug.LogWarning($"Ya existe {nombre} en el inventario");
            return;
        }

        int cantidadNum = 1;
        if (inputCantidad != null && !string.IsNullOrEmpty(inputCantidad.text))
        {
            int.TryParse(inputCantidad.text, out cantidadNum);
        }

        ItemInventario nuevoItem = new ItemInventario
        {
            id = Guid.NewGuid().ToString(),
            nombre = nombre,
            cantidad = cantidadNum,
            categoria = dropdownCategoria != null
                        ? dropdownCategoria.options[dropdownCategoria.value].text
                        : "General"
        };

        inventarioDB.items.Add(nuevoItem);
        GuardarInventario();
        MostrarInventario();

        if (inputNombre) inputNombre.text = "";
        if (inputCantidad) inputCantidad.text = "1";
    }

    // =====================================================
    public void MostrarInventario()
    {
        if (contentLista == null || itemPrefab == null)
        {
            Debug.Log("📦 Inventario cargado, pero sin asignar UI en esta escena.");
            return;
        }

        // Limpiar lista actual
        for (int i = contentLista.childCount - 1; i >= 0; i--)
            Destroy(contentLista.GetChild(i).gameObject);

        foreach (var item in inventarioDB.items)
        {
            GameObject card = Instantiate(itemPrefab, contentLista);

            TMP_Text txtNombre = card.transform.Find("TextNombre")?.GetComponent<TMP_Text>();
            TMP_Text txtCantidad = card.transform.Find("TextCantidad")?.GetComponent<TMP_Text>();
            TMP_Text txtCategoria = card.transform.Find("TextCategoria")?.GetComponent<TMP_Text>();

            if (txtNombre) txtNombre.text = item.nombre;
            if (txtCantidad) txtCantidad.text = $"Cantidad: {item.cantidad}";
            if (txtCategoria) txtCategoria.text = item.categoria;

            Button btnMas = card.transform.Find("BtnMas")?.GetComponent<Button>();
            Button btnMenos = card.transform.Find("BtnMenos")?.GetComponent<Button>();
            Button btnEliminar = card.transform.Find("BtnEliminar")?.GetComponent<Button>();

            string itemId = item.id;
            if (btnMas) btnMas.onClick.AddListener(() => ModificarCantidad(itemId, 1));
            if (btnMenos) btnMenos.onClick.AddListener(() => ModificarCantidad(itemId, -1));
            if (btnEliminar) btnEliminar.onClick.AddListener(() => EliminarItem(itemId));
        }
    }

    // =====================================================
    void ModificarCantidad(string id, int cambio)
    {
        var item = inventarioDB.items.Find(i => i.id == id);
        if (item != null)
        {
            item.cantidad += cambio;
            if (item.cantidad < 0) item.cantidad = 0;

            GuardarInventario();
            MostrarInventario();
        }
    }

    void EliminarItem(string id)
    {
        inventarioDB.items.RemoveAll(i => i.id == id);
        GuardarInventario();
        MostrarInventario();
    }

    // =====================================================
    public int GetCantidadDisponible(string nombreHerramienta)
    {
        var item = inventarioDB.items.Find(i => i.nombre.ToLower() == nombreHerramienta.ToLower());
        return item != null ? item.cantidad : 0;
    }

    public void ActualizarCantidad(string nombreHerramienta, int cambio)
    {
        var item = inventarioDB.items.Find(i => i.nombre.ToLower() == nombreHerramienta.ToLower());
        if (item != null)
        {
            item.cantidad += cambio;
            if (item.cantidad < 0) item.cantidad = 0;

            GuardarInventario();
            MostrarInventario();
        }
    }
}