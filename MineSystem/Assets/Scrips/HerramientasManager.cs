using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

[System.Serializable]
public class ToolItem
{
    public string nombre;
    public string estado;
    public string fechaHora;
    public string fechaPrestamo;
    public string fechaDevolucion;
    public string uniqueId;
}

[System.Serializable]
public class ToolEntry
{
    public string id;
    public List<ToolItem> items = new List<ToolItem>();
}

[System.Serializable]
public class ToolDatabase
{
    public List<ToolEntry> entries = new List<ToolEntry>();
}

public class HerramientasManager : MonoBehaviour
{
    [Header("UI")]
    public Transform content;
    [Header("Contenedores de listas")]
    public Transform contentPrestados;
    public Transform contentDevueltos;
    [Header("Paneles UI")]
    public GameObject panelPrestadas;
    public GameObject panelDevueltas;

    public GameObject itemCardPrefab;
    public TMP_InputField inputID;
    public TMP_InputField inputNombre;

    private string filePath;
    private ToolDatabase db = new ToolDatabase();
    private InventarioManager inventarioManager; // Para conectar con inventario

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "herramientas.json");
    }

    void Start()
    {
        LoadDatabase();
        ShowAll();
        InvokeRepeating("ActualizarTiempos", 60f, 60f);

        // Buscar el InventarioManager persistente
        inventarioManager = FindFirstObjectByType<InventarioManager>();
        if (inventarioManager == null)
        {
            Debug.LogWarning("⚠️ InventarioManager no encontrado - Los préstamos no verificarán inventario");
        }
        else
        {
            Debug.Log("✅ InventarioManager encontrado y conectado");
        }
    }

    // -------- Cargar y guardar --------
    void LoadDatabase()
    {
        if (!File.Exists(filePath)) { db = new ToolDatabase(); return; }
        string raw = File.ReadAllText(filePath);
        db = JsonUtility.FromJson<ToolDatabase>(raw);
        if (db == null || db.entries == null) db = new ToolDatabase();
    }

    void SaveDatabase()
    {
        string json = JsonUtility.ToJson(db, true);
        File.WriteAllText(filePath, json);
    }

    // -------- Mostrar tarjetas --------
    public void ShowAll()
    {
        ClearContent(contentPrestados);
        ClearContent(contentDevueltos);

        foreach (var entry in db.entries)
        {
            foreach (var item in entry.items)
            {
                if (item.estado == "Prestada")
                    CreateCard(entry.id, item, contentPrestados);
                else
                    CreateCard(entry.id, item, contentDevueltos);
            }
        }
    }

    void ClearContent(Transform contenedor)
    {
        for (int i = contenedor.childCount - 1; i >= 0; i--)
            Destroy(contenedor.GetChild(i).gameObject);
    }

    // -------- Calcular tiempo de uso --------
    string CalcularTiempoUso(string fechaInicio, string fechaFin = "")
    {
        try
        {
            DateTime inicio = DateTime.Parse(fechaInicio);
            DateTime fin = string.IsNullOrEmpty(fechaFin) ? DateTime.Now : DateTime.Parse(fechaFin);
            TimeSpan diferencia = fin - inicio;

            if (diferencia.TotalDays >= 1)
            {
                return $"{(int)diferencia.TotalDays}d {diferencia.Hours}h";
            }
            else if (diferencia.TotalHours >= 1)
            {
                return $"{(int)diferencia.TotalHours}h {diferencia.Minutes}m";
            }
            else
            {
                return $"{diferencia.Minutes}m";
            }
        }
        catch
        {
            return "0m";
        }
    }

    // -------- Saldar préstamo --------
    public void SaldarPrestamoUnico(string id, string uniqueId)
    {
        var entry = db.entries.Find(e => e.id == id);
        if (entry == null)
        {
            Debug.LogWarning("No se encontró la herramienta con ID: " + id);
            return;
        }

        var item = entry.items.Find(i => i.uniqueId == uniqueId);
        if (item != null && item.estado == "Prestada")
        {
            item.estado = "Devuelta";
            item.fechaDevolucion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            item.fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Devolver al inventario
            if (inventarioManager != null)
            {
                inventarioManager.ActualizarCantidad(item.nombre, 1);
                Debug.Log($"✅ {item.nombre} devuelta al inventario");
            }

            string tiempoTotal = CalcularTiempoUso(item.fechaPrestamo, item.fechaDevolucion);
            Debug.Log($"✅ {item.nombre} devuelta. Tiempo de uso: {tiempoTotal}");
        }

        SaveDatabase();
        ShowAll();
    }

    // -------- Crear tarjetas --------
    void CreateCard(string id, ToolItem item, Transform parent)
    {
        GameObject card = Instantiate(itemCardPrefab, parent);

        if (string.IsNullOrEmpty(item.uniqueId))
        {
            item.uniqueId = System.Guid.NewGuid().ToString();
            SaveDatabase();
        }

        Debug.Log($"=== Creando tarjeta para {item.nombre} ===");
        Debug.Log($"ID: {id}, UniqueID: {item.uniqueId}");
        Debug.Log($"Estado: {item.estado}");

        CardData cardData = card.AddComponent<CardData>();
        cardData.InicializarTarjeta(id, item.uniqueId, this);

        TMP_Text txtNombre = card.transform.Find("TextNombre")?.GetComponent<TMP_Text>();
        TMP_Text txtID = card.transform.Find("TextID")?.GetComponent<TMP_Text>();
        //TMP_Text txtTXT = card.transform.Find("TXT")?.GetComponent<TMP_Text>();
        TMP_Text txtFecha = card.transform.Find("TextFecha")?.GetComponent<TMP_Text>();
        TMP_Dropdown drop = card.transform.Find("DropdownEstado")?.GetComponent<TMP_Dropdown>();

        if (txtNombre) txtNombre.text = item.nombre;
        if (txtID) txtID.text = id;
       // if (txtTXT) txtTXT.text = id;

        if (txtFecha)
        {
            if (item.estado == "Prestada")
            {
                if (!string.IsNullOrEmpty(item.fechaPrestamo))
                {
                    string tiempoUso = CalcularTiempoUso(item.fechaPrestamo);
                    DateTime fechaPrest = DateTime.Parse(item.fechaPrestamo);
                    txtFecha.text = $"Prestado: {fechaPrest.ToString("dd/MM HH:mm")}\nEn uso: {tiempoUso}";
                }
                else
                {
                    txtFecha.text = $"Fecha: {item.fechaHora}";
                }

                // Mostrar disponibilidad del inventario
                if (inventarioManager != null)
                {
                    int disponible = inventarioManager.GetCantidadDisponible(item.nombre);
                    txtFecha.text += $"\n📦 Disponibles: {disponible}";
                }
            }
            else if (item.estado == "Devuelta")
            {
                if (!string.IsNullOrEmpty(item.fechaPrestamo) && !string.IsNullOrEmpty(item.fechaDevolucion))
                {
                    string tiempoTotal = CalcularTiempoUso(item.fechaPrestamo, item.fechaDevolucion);
                    DateTime fechaDev = DateTime.Parse(item.fechaDevolucion);
                    txtFecha.text = $"Devuelto: {fechaDev.ToString("dd/MM HH:mm")}\nTiempo usado: {tiempoTotal}";
                }
                else
                {
                    txtFecha.text = $"Devuelto: {item.fechaHora}";
                }
            }
            else
            {
                txtFecha.text = $"Fecha: {item.fechaHora}";
            }
        }

        if (drop)
        {
            if (drop.options.Count == 0)
            {
                drop.options.Add(new TMP_Dropdown.OptionData("Prestado"));
                drop.options.Add(new TMP_Dropdown.OptionData("Devuelto"));
            }

            if (item.estado == "Prestada" || item.estado == "Prestado")
            {
                drop.value = 0;
            }
            else if (item.estado == "Devuelta" || item.estado == "Devuelto")
            {
                drop.value = 1;
            }

            drop.onValueChanged.RemoveAllListeners();
            drop.onValueChanged.AddListener((val) =>
            {
                item.estado = val == 0 ? "Prestada" : "Devuelta";

                if (val == 1)
                {
                    item.fechaDevolucion = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (string.IsNullOrEmpty(item.fechaPrestamo))
                    {
                        item.fechaPrestamo = item.fechaHora;
                    }

                    if (inventarioManager != null)
                    {
                        inventarioManager.ActualizarCantidad(item.nombre, 1);
                    }
                }
                else if (val == 0)
                {
                    item.fechaPrestamo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    item.fechaDevolucion = "";

                    if (inventarioManager != null)
                    {
                        inventarioManager.ActualizarCantidad(item.nombre, -1);
                    }
                }

                item.fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                SaveDatabase();
                ShowAll();
            });
        }

        GameObject fechaPrestamoObj = card.transform.Find("fechaPrestamo")?.gameObject;
        GameObject fechaDevolucionObj = card.transform.Find("fechaDevolucion")?.gameObject;
        if (fechaPrestamoObj) fechaPrestamoObj.SetActive(false);
        if (fechaDevolucionObj) fechaDevolucionObj.SetActive(false);

        Button btnSaldar = card.transform.Find("BotonSaldar")?.GetComponent<Button>();
        if (btnSaldar)
        {
            btnSaldar.gameObject.SetActive(item.estado == "Prestada");
        }
    }

    // -------- Agregar nueva herramienta --------
    public void AddItemFromUI()
    {
        string id = inputID.text.Trim();
        string nombre = inputNombre.text.Trim();

        Debug.Log($"=== INTENTANDO PRESTAR ===");
        Debug.Log($"ID: {id}, Nombre: {nombre}");

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(nombre))
        {
            Debug.LogWarning("ID o nombre vacío.");
            return;
        }

        // Verificar inventario
        if (inventarioManager != null)
        {
            Debug.Log("✅ InventarioManager encontrado");

            int disponible = inventarioManager.GetCantidadDisponible(nombre);
            Debug.Log($"Cantidad disponible de {nombre}: {disponible}");

            if (disponible <= 0)
            {
                Debug.LogWarning($"❌ No hay {nombre} disponible en el inventario");
                return;
            }

            inventarioManager.ActualizarCantidad(nombre, -1);
            Debug.Log($"✅ Prestando {nombre}. Quedan {disponible - 1} disponibles");
        }
        else
        {
            Debug.LogWarning("❌ No se encontró InventarioManager - El préstamo continuará sin verificación");
        }

        ToolItem t = new ToolItem
        {
            nombre = nombre,
            estado = "Prestada",
            fechaHora = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            fechaPrestamo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            fechaDevolucion = "",
            uniqueId = System.Guid.NewGuid().ToString()
        };

        ToolEntry entry = db.entries.Find(e => e.id == id);
        if (entry == null)
        {
            entry = new ToolEntry { id = id };
            db.entries.Add(entry);
        }
        entry.items.Add(t);

        SaveDatabase();
        ShowAll();

        inputID.text = "";
        inputNombre.text = "";

        Debug.Log($"=== PRÉSTAMO COMPLETADO ===");
    }

    // -------- Actualizar tiempos automáticamente --------
    void ActualizarTiempos()
    {
        bool hayPrestadas = db.entries.Any(e => e.items.Any(i => i.estado == "Prestada"));
        if (hayPrestadas)
        {
            ShowAll();
            Debug.Log("⏱️ Tiempos actualizados");
        }
    }

    // -------- Borrar to

    // -------- Borrar todo --------
    public void ClearAndDeleteFile()
    {
        db = new ToolDatabase();
        SaveDatabase();
        ClearContent(contentPrestados);
        ClearContent(contentDevueltos);
        Debug.Log("🗑️ Base de datos limpiada");
    }

    // -------- Cambiar paneles --------
    public void MostrarPrestadas()
    {
        panelPrestadas.SetActive(true);
        panelDevueltas.SetActive(false);
    }

    public void MostrarDevueltas()
    {
        panelPrestadas.SetActive(false);
        panelDevueltas.SetActive(true);
    }
}