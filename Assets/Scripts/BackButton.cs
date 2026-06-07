using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public Button backButton;

    // Start is called before the first frame update
    void Start()
    {
        backButton.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });
    }

}
