using UnityEngine;
using TMPro;

public class PriceUIController : MonoBehaviour
{
    public TMP_InputField inputOro;
    public TMP_InputField inputCombustible;

    private void Start()
    {
        inputOro.text = PriceManager.Instance.precios.precioOro.ToString();
        inputCombustible.text = PriceManager.Instance.precios.precioCombustible.ToString();
    }

    public void ActualizarPrecios()
    {
        float.TryParse(inputOro.text, out PriceManager.Instance.precios.precioOro);
        float.TryParse(inputCombustible.text, out PriceManager.Instance.precios.precioCombustible);
    }
}
