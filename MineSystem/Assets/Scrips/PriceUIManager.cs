using UnityEngine;
using TMPro;

public class PriceUIManager : MonoBehaviour
{
    public TMP_InputField fuelPriceInput;
    public TMP_InputField goldPriceInput;
    public TMP_InputField goldPerHourInput;

    void Start()
    {
        if (PriceManager.Instance != null)
        {
            fuelPriceInput.text = PriceManager.Instance.GetFuelPrice().ToString("F2");
            goldPriceInput.text = PriceManager.Instance.GetGoldPrice().ToString("F2");
            goldPerHourInput.text = PriceManager.Instance.GetGoldPerHour().ToString("F2");
        }
    }

    public void OnFuelPriceChanged(string text)
    {
        if (float.TryParse(text, out float v)) PriceManager.Instance.SetFuelPrice(v);
    }
    public void OnGoldPriceChanged(string text)
    {
        if (float.TryParse(text, out float v)) PriceManager.Instance.SetGoldPrice(v);
    }
    public void OnGoldPerHourChanged(string text)
    {
        if (float.TryParse(text, out float v)) PriceManager.Instance.SetGoldPerHour(v);
    }
}
