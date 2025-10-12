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

    // 🔹 Nuevo campo: costo de operación basado en el precio de gasolina al registrar
    public float costoOperacion;
}

[System.Serializable]
public class DatabaseInventario
{
    public List<ItemInventario> items = new List<ItemInventario>();
}

public class InventarioManager : MonoBehaviour
{
    [Header("UI Referencias")]
    public Transform contentLista;          // El Content del ScrollView
    public GameObject itemPrefab;           // Prefab para mostrar cada item

    [Header("Inputs")]
    public TMP_InputField inputNombre;
    public TMP_InputField inputCantidad;
    public TMP_Dropdown dropdownCategoria;

    [Header("Referencias externas")]
    public GasolinaManager gasolinaManager; // 🔹 referencia al manejador de gasolina

    private string inventarioPath;
    private DatabaseInventario inventarioDB = new DatabaseInventario();

    void Awake()
    {
        inventarioPath = Path.Combine(Application.persistentDataPath, "inventario_mina.json");
    }

    void Start()
    {
        // Configurar dropdown (si está vacío)
        if (dropdownCategoria != null && dropdownCategoria.options.Count == 0)
        {
            dropdownCategoria.options.Add(new TMP_Dropdown.OptionData("HERRAMIENTA"));
            dropdownCategoria.options.Add(new TMP_Dropdown.OptionData("COMIDA"));
        }

        CargarInventario();
        MostrarInventario();
    }

    void CargarInventario()
    {
        if (!File.Exists(inventarioPath)) return;

        string json = File.ReadAllText(inventarioPath);
        inventarioDB = JsonUtility.FromJson<DatabaseInventario>(json);
        if (inventarioDB == null) inventarioDB = new DatabaseInventario();
    }

    void GuardarInventario()
    {
        string json = JsonUtility.ToJson(inventarioDB, true);
        File.WriteAllText(inventarioPath, json);
    }

    public void AgregarItem()
    {
        // Validaciones básicas
        string nombre = inputNombre != null ? inputNombre.text.Trim() : "";
        if (string.IsNullOrEmpty(nombre))
        {
            Debug.LogWarning("Nombre vacío. No se agrega el item.");
            return;
        }

        if (inputCantidad == null)
        {
            Debug.LogWarning("InputCantidad no está asignado en el inspector.");
            return;
        }

        if (!int.TryParse(inputCantidad.text, out int cantidad))
        {
            Debug.LogWarning("Cantidad inválida. Usa un número entero.");
            return;
        }

        string categoria = "SIN_CATEGORIA";
        if (dropdownCategoria != null && dropdownCategoria.options.Count > 0)
            categoria = dropdownCategoria.options[dropdownCategoria.value].text;

        // Comprobar gasolinaManager
        if (gasolinaManager == null)
        {
            Debug.LogWarning("GasolinaManager no asignado. Usando precio 0.");
        }
        float precioGasolina = gasolinaManager != null ? gasolinaManager.precioGasolina : 0f;

        // 🔹 Calcular el costo de operación usando el valor actual de gasolina
        float costoOperacion = precioGasolina * cantidad;

        // Crear y añadir el item correctamente (comas entre campos)
        ItemInventario nuevoItem = new ItemInventario
        {
            id = Guid.NewGuid().ToString(),
            nombre = nombre,
            cantidad = cantidad,
            categoria = categoria,
            costoOperacion = costoOperacion // 🔹 Guardar costo fijo
        };

        inventarioDB.items.Add(nuevoItem);
        GuardarInventario();
        MostrarInventario();

        // Limpiar campos
        inputNombre.text = "";
        inputCantidad.text = "1";
    }

    public void MostrarInventario()
    {
        if (contentLista == null) return;

        // Limpiar lista actual
        for (int i = contentLista.childCount - 1; i >= 0; i--)
            Destroy(contentLista.GetChild(i).gameObject);

        // Mostrar cada item
        foreach (var item in inventarioDB.items)
        {
            GameObject card = Instantiate(itemPrefab, contentLista);

            // Asignar textos
            TMP_Text txtNombre = card.transform.Find("TextNombre")?.GetComponent<TMP_Text>();
            TMP_Text txtCantidad = card.transform.Find("TextCantidad")?.GetComponent<TMP_Text>();
            TMP_Text txtCategoria = card.transform.Find("TextCategoria")?.GetComponent<TMP_Text>();
            TMP_Text txtCosto = card.transform.Find("TextCosto")?.GetComponent<TMP_Text>();

            if (txtCosto) txtCosto.text = $"Costo: ${item.costoOperacion:F2}";
            if (txtNombre) txtNombre.text = item.nombre;
            if (txtCantidad) txtCantidad.text = $"Cantidad: {item.cantidad}";
            if (txtCategoria) txtCategoria.text = item.categoria;

            // Botones
            Button btnMas = card.transform.Find("BtnMas")?.GetComponent<Button>();
            Button btnMenos = card.transform.Find("BtnMenos")?.GetComponent<Button>();
            Button btnEliminar = card.transform.Find("BtnEliminar")?.GetComponent<Button>();

            string itemId = item.id;

            if (btnMas) btnMas.onClick.AddListener(() => ModificarCantidad(itemId, 1));
            if (btnMenos) btnMenos.onClick.AddListener(() => ModificarCantidad(itemId, -1));
            if (btnEliminar) btnEliminar.onClick.AddListener(() => EliminarItem(itemId));
        }
    }

    void ModificarCantidad(string id, int cambio)
    {
        var item = inventarioDB.items.Find(i => i.id == id);
        if (item != null)
        {
            item.cantidad += cambio;
            if (item.cantidad < 0) item.cantidad = 0;

            // Si quieres que el costo cambie cuando cambie la cantidad, descomenta y ajusta:
            // item.costoOperacion = (gasolinaManager != null ? gasolinaManager.precioGasolina : 0f) * item.cantidad;

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

    // MÉTODOS PARA CONECTAR CON HERRAMIENTAS

    // Obtener cantidad disponible de una herramienta
    public int GetCantidadDisponible(string nombreHerramienta)
    {
        var item = inventarioDB.items.Find(i => i.nombre.ToLower() == nombreHerramienta.ToLower());
        if (item == null) return 0;
        return item.cantidad;
    }

    // Actualizar cantidad cuando se presta o devuelve
    public void ActualizarCantidad(string nombreHerramienta, int cambio)
    {
        var item = inventarioDB.items.Find(i => i.nombre.ToLower() == nombreHerramienta.ToLower());
        if (item != null)
        {
            item.cantidad += cambio;
            if (item.cantidad < 0) item.cantidad = 0; // No permitir negativos

            GuardarInventario();
            MostrarInventario();
        }
    }
}
