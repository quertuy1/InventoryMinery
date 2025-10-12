using UnityEngine;
using TMPro;

public class GasolinaManager : MonoBehaviour
{
    [Header("UI Referencias")]
    public TMP_InputField inputPrecioGasolina; // Campo de entrada
    public TMP_Text textoPrecioActual;         // Texto que muestra el valor actual

    [Header("Configuración")]
    public float precioGasolina = 0f;

    private const string KEY_PRECIO_GASOLINA = "precio_gasolina";

    void Start()
    {
        // Cargar el precio guardado si existe
        if (PlayerPrefs.HasKey(KEY_PRECIO_GASOLINA))
        {
            precioGasolina = PlayerPrefs.GetFloat(KEY_PRECIO_GASOLINA);
        }

        ActualizarUI();

        // 🔹 Actualiza en tiempo real cuando el usuario escribe
        if (inputPrecioGasolina != null)
            inputPrecioGasolina.onValueChanged.AddListener(OnPrecioCambiado);
    }

    private void OnPrecioCambiado(string texto)
    {
        if (float.TryParse(texto, out float nuevoPrecio))
        {
            precioGasolina = nuevoPrecio;
            PlayerPrefs.SetFloat(KEY_PRECIO_GASOLINA, precioGasolina);
            PlayerPrefs.Save();
            ActualizarUI();
        }
    }

    private void ActualizarUI()
    {
        if (textoPrecioActual != null)
            textoPrecioActual.text = $"Precio actual: ${precioGasolina:F2}";
    }
}
