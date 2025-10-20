
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{
    // 🔹 Cargar una escena por nombre
    public void CargarEscenaPorNombre(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // 🔹 Cargar una escena por número (índice del Build Settings)
    public void CargarEscenaPorIndice(int indice)
    {
        SceneManager.LoadScene(indice);
    }

    // 🔹 Recargar la escena actual
    public void RecargarEscenaActual()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 🔹 Cerrar la aplicación (para builds)
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
