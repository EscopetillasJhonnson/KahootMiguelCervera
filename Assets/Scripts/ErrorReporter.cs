using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ErrorReporter : MonoBehaviour
{
    private static string logPath = Application.persistentDataPath + "/error_log.txt";

    public static void Report(string message)
    {
        // Mostrar en consola
        Debug.LogError("[ERROR] " + message);

        // Guardar en archivo
        try
        {
            File.AppendAllText(logPath, System.DateTime.Now + " - " + message + "\n");
        }
        catch (System.Exception e)
        {
            Debug.LogError("No se pudo escribir en el log: " + e.Message);
        }
    }

    public static void ReportToUser(string message, TMPro.TMP_Text uiText)
    {
        // Mostrar en pantalla al jugador
        if (uiText != null)
        {
            uiText.text = "⚠️ " + message;
        }

        // También lo guardamos en log
        Report(message);
    }
}
