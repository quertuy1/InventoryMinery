using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardData : MonoBehaviour
{
    private string idHerramienta;
    private string uniqueId; // NUEVO: ID único para cada préstamo
    private HerramientasManager manager;
    private TMP_Dropdown dropdown;

    // Método actualizado con 3 parámetros
    public void InicializarTarjeta(string id, string unique, HerramientasManager mgr)
    {
        idHerramienta = id;
        uniqueId = unique; // Guardar el ID único
        manager = mgr;

        // Buscar el dropdown en esta tarjeta
        dropdown = transform.Find("DropdownEstado")?.GetComponent<TMP_Dropdown>();

        Debug.Log($"✅ Tarjeta inicializada - ID: {idHerramienta}, UniqueID: {uniqueId}");

        Button botonSaldar = transform.Find("BotonSaldar")?.GetComponent<Button>();

        if (botonSaldar != null)
        {
            botonSaldar.onClick.RemoveAllListeners();
            botonSaldar.onClick.AddListener(OnClickSaldar);
            Debug.Log($"✅ Botón configurado para ID: {idHerramienta}");
        }
    }

    void OnClickSaldar()
    {
        Debug.Log($"🔘 Click en botón - ID: {idHerramienta}, UniqueID: {uniqueId}");

        if (manager != null && !string.IsNullOrEmpty(idHerramienta) && !string.IsNullOrEmpty(uniqueId))
        {
            // Actualizar el dropdown visualmente antes de llamar a SaldarPrestamo
            if (dropdown != null)
            {
                dropdown.value = 1; // Cambiar a "Devuelto"
            }

            // Llamar al método que salda un préstamo específico
            manager.SaldarPrestamoUnico(idHerramienta, uniqueId);
        }
        else
        {
            Debug.LogError("❌ Falta información para saldar el préstamo");
        }
    }
}