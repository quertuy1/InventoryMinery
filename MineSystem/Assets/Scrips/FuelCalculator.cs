using UnityEngine;

public class FuelCalculator : MonoBehaviour
{
    // Convierte litros a galones y multiplica por precio actual del galón
    public float CalcularCostoPorLitros(float litros)
    {
        if (PriceManager.Instance == null) return 0f;
        float precioPorGalon = PriceManager.Instance.GetFuelPrice();
        float galones = litros / 3.785411784f; // conversión litros -> galones (US)
        return galones * precioPorGalon;
    }

    public float CalcularCostoPorGalones(float galones)
    {
        if (PriceManager.Instance == null) return 0f;
        float precioPorGalon = PriceManager.Instance.GetFuelPrice();
        return galones * precioPorGalon;
    }
}
