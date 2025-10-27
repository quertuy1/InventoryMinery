using UnityEngine;

public class FuelCalculator : MonoBehaviour
{
    // calcula costo a partir de litros (usa precio por galón o por litro según PriceManager)
    // asumimos precio en la misma unidad (precio por litro)
    public float CalcularCostoPorLitros(float litros)
    {
        if (PriceManager.Instance == null) return 0f;
        return litros * PriceManager.Instance.GetFuelPrice();
    }

    public float CalcularCostoPorGalones(float galones)
    {
        // si usas galones convierte a litros si PriceManager está en litros; aquí asumimos precio por litro
        float litros = galones * 3.785411784f;
        return CalcularCostoPorLitros(litros);
    }
}
