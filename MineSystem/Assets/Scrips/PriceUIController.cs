using UnityEngine;
using TMPro;

public class PriceUIController : MonoBehaviour
{
    public TMP_Text fuelText;
    public TMP_Text goldText;
    public TMP_Text goldPerHourText;

    void Update()
    {
        if (PriceManager.Instance == null) return;
        fuelText.text = $"Combustible: ${PriceManager.Instance.GetFuelPrice():F2}/gal";
        goldText.text = $"Oro: ${PriceManager.Instance.GetGoldPrice():F2}/u";
        goldPerHourText.text = $"Prod: {PriceManager.Instance.GetGoldPerHour():F2} u/h";
    }
}
