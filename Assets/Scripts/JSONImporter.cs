using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSONImporter : MonoBehaviour
{
    public GameObject notificationPanel;
    public TMP_Text notificationText;

    private string kahootPath => Path.Combine(Application.persistentDataPath, "Kahoots");
    private string[] lastFiles;

    void Start()
    {
        if (!Directory.Exists(kahootPath))
            Directory.CreateDirectory(kahootPath);

        // Guardar estado inicial
        lastFiles = Directory.GetFiles(kahootPath, "*.json");

        // Ocultar panel al inicio
        notificationPanel.SetActive(false);

        // Revisar cada 2 segundos si hay cambios
        InvokeRepeating(nameof(CheckForNewFiles), 2f, 2f);
    }

    void CheckForNewFiles()
    {
        string[] currentFiles = Directory.GetFiles(kahootPath, "*.json");

        // Detectar si hay más archivos que antes
        if (currentFiles.Length > lastFiles.Length)
        {
            // Buscar cuál es nuevo
            foreach (string file in currentFiles)
            {
                if (System.Array.IndexOf(lastFiles, file) == -1)
                {
                    string fileName = Path.GetFileName(file);

                    // Refrescar lista de Kahoots
                    FindObjectOfType<KahootSelector>().RefreshKahootList();

                    // Mostrar notificación
                    ShowNotification("Nuevo Kahoot importado: " + fileName);
                }
            }
        }

        // Actualizar referencia
        lastFiles = currentFiles;
    }

    void ShowNotification(string message)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);
        Invoke(nameof(HideNotification), 3f);
    }

    void HideNotification()
    {
        notificationPanel.SetActive(false);
    }
}
