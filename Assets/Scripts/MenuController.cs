using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
   
    public void CargarEscena(string nombreEscena)
    {
        if (!string.IsNullOrEmpty(nombreEscena))
        {
            SceneManager.LoadScene(nombreEscena);
        }
        else
        {
            Debug.LogWarning("Nombre de escena vacío o nulo.");
        }
    }

    public void SalirDelJuego()
    {
        Debug.Log(Application.persistentDataPath);
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
