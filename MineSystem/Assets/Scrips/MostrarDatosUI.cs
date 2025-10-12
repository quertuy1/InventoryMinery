using UnityEngine;
using TMPro;

public class MostrarDatosUI : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TMP_InputField nombreInput;      // Campo de texto para el nombre
    public TMP_Dropdown estadoDropdown;     // Dropdown para el estado
    public Transform content;               // ScrollView/Viewport/Content
    public GameObject itemPrefabTMP;        // Prefab con un TMP_Text

    // Llama este método desde el botón "Agregar"
    public void AgregarItem()
    {
        // 1️⃣ Obtener datos de los campos
        string nombre = nombreInput.text;
        string estado = estadoDropdown.options[estadoDropdown.value].text;
        string fechaHora = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        // 2️⃣ Instanciar el prefab en el Content del ScrollView
        GameObject nuevoItem = Instantiate(itemPrefabTMP, content);

        // 3️⃣ Asignar el texto formateado al TMP_Text del prefab
        TMP_Text textoItem = nuevoItem.GetComponent<TMP_Text>();
        textoItem.text = $"{nombre} | {estado} | {fechaHora}";

        // (Opcional) limpiar el campo de nombre para la próxima entrada
        nombreInput.text = "";
    }
}
