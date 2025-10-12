using System;
using UnityEngine;

public static class TimeCalculator
{
    public static string CalcularTiempoTranscurrido(string fechaInicio)
    {
        try
        {
            DateTime inicio = DateTime.Parse(fechaInicio);
            DateTime ahora = DateTime.Now;
            TimeSpan diferencia = ahora - inicio;

            if (diferencia.TotalDays >= 1)
            {
                int dias = (int)diferencia.TotalDays;
                int horas = diferencia.Hours;
                return $"{dias}d {horas}h";
            }
            else if (diferencia.TotalHours >= 1)
            {
                int horas = (int)diferencia.TotalHours;
                int minutos = diferencia.Minutes;
                return $"{horas}h {minutos}m";
            }
            else
            {
                return $"{diferencia.Minutes}m";
            }
        }
        catch
        {
            return "N/A";
        }
    }

    public static string ObtenerTiempoTotal(string fechaPrestamo, string fechaDevolucion)
    {
        try
        {
            DateTime inicio = DateTime.Parse(fechaPrestamo);
            DateTime fin = DateTime.Parse(fechaDevolucion);
            TimeSpan total = fin - inicio;

            if (total.TotalDays >= 1)
                return $"{(int)total.TotalDays}d {total.Hours}h {total.Minutes}m";
            else
                return $"{(int)total.TotalHours}h {total.Minutes}m";
        }
        catch
        {
            return "N/A";
        }
    }

    public static bool EstaVencida(string fechaPrestamo, int diasLimite)
    {
        try
        {
            DateTime inicio = DateTime.Parse(fechaPrestamo);
            return (DateTime.Now - inicio).TotalDays > diasLimite;
        }
        catch
        {
            return false;
        }
    }
}