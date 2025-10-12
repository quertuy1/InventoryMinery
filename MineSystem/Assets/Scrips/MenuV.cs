using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuV : MonoBehaviour
{
    public void AbrirMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
