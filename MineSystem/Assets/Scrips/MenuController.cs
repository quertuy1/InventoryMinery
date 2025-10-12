using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void AbrirHerramientas()
    {
        SceneManager.LoadScene("Herramientas");
    }

    public void AbrirComida()
    {
        SceneManager.LoadScene("Comida");
    }

    public void AbrirMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
