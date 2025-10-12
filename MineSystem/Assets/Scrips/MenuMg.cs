using UnityEngine;

public class MenuMg : MonoBehaviour
{
    // Aquí puedes definir las herramientas o menús que quieras controlar
    public GameObject menuHerramientas;

    // Abre el menú de herramientas
    public void AbrirHerramientas()
    {
        if (menuHerramientas != null)
        {
            menuHerramientas.SetActive(true);
        }
    }

    // Cierra el menú de herramientas
    public void CerrarHerramientas()
    {
        if (menuHerramientas != null)
        {
            menuHerramientas.SetActive(false);
        }
    }

    // Alternar entre abrir y cerrar
    public void ToggleHerramientas()
    {
        if (menuHerramientas != null)
        {
            menuHerramientas.SetActive(!menuHerramientas.activeSelf);
        }
    }
}
