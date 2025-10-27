using UnityEngine;
using System;

[Serializable]
public class PriceData
{
    public float precioOro = 100f;
    public float precioCombustible = 10f;
    // opcional: puedes agregar más campos aquí
}

public class PriceManager : MonoBehaviour
{
    public static PriceManager Instance { get; private set; }

    public PriceData precios = new PriceData();

    // Valor adicional: oro generado por hora (por máquina u operación)
    [Header("Valores dinámicos")]
    public float oroPorHora = 1f;       // usado por algunos UIs o calculadores
    public float combustiblePorUnidad = 1f; // si necesitas alguna unidad extra

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Fuel price (precio por litro, por ejemplo) ---
    public float GetFuelPrice() => precios.precioCombustible;
    public void SetFuelPrice(float p) => precios.precioCombustible = p;

    // aliases para compatibilidad con nombres anteriores
    public float ObtenerPrecioCombustible() => GetFuelPrice();
    public void SetPrecioCombustible(float p) => SetFuelPrice(p);

    // --- Gold price (precio del oro por unidad monetaria, si aplica) ---
    public float GetGoldPrice() => precios.precioOro;
    public void SetGoldPrice(float p) => precios.precioOro = p;

    // --- Oro por hora (valor que algunos UIs referenciaban) ---
    public float GetGoldPerHour() => oroPorHora;
    public void SetGoldPerHour(float v) => oroPorHora = v;

    // aliases por si se usaban nombres en español
    public float GetOroPorHora() => GetGoldPerHour();
    public void SetOroPorHora(float v) => SetGoldPerHour(v);
}
