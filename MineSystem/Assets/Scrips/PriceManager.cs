
using UnityEngine;

public class PriceManager : MonoBehaviour
{
    public static PriceManager Instance { get; private set; }

    [Header("Precios globales")]
    [Tooltip("Precio del combustible por galón")]
    public float fuelPricePerGallon = 3.5f;

    [Tooltip("Precio del oro por unidad (ej: gramo)")]
    public float goldPricePerUnit = 150000f;

    [Tooltip("Oro producido por hora por defecto (unidad/ hora)")]
    public float goldPerHour = 3.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Getters
    public float GetFuelPrice() => fuelPricePerGallon;
    public float GetGoldPrice() => goldPricePerUnit;
    public float GetGoldPerHour() => goldPerHour;

    // Setters (para UI)
    public void SetFuelPrice(float v) { fuelPricePerGallon = v; Debug.Log($"PriceManager: fuelPricePerGallon = {v}"); }
    public void SetGoldPrice(float v) { goldPricePerUnit = v; Debug.Log($"PriceManager: goldPricePerUnit = {v}"); }
    public void SetGoldPerHour(float v) { goldPerHour = v; Debug.Log($"PriceManager: goldPerHour = {v}"); }

    // Alias/compatibility (si otros scripts usan otros nombres)
    public float GetPrecioCombustible() => GetFuelPrice();
    public void UpdateFuelPrice(float v) => SetFuelPrice(v);
    public float GetPrecioOro() => GetGoldPrice();
    public void UpdateGoldPrice(float v) => SetGoldPrice(v);
}
