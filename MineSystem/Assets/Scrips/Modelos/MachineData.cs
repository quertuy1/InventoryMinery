using System;

[Serializable]
public class MachineData
{
    public string nombreMaquina;
    public float litrosPorHora;
    public float oroPorHora;
    public float combustibleActual;

    public MachineData() { }

    public MachineData(string nombre, float litrosHora, float oroHora, float combustible)
    {
        nombreMaquina = nombre;
        litrosPorHora = litrosHora;
        oroPorHora = oroHora;
        combustibleActual = combustible;
    }
}
