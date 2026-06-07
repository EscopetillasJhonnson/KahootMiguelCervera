using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ErrorReportManager : MonoBehaviour
{
    public Transform reportContainer;
    public GameObject reportEntryPrefab;
    public TMP_Text titleText;

    private string logPath;

    void Start()
    {
        logPath = Application.persistentDataPath + "/error_log.txt";

        titleText.text = "Informes";

        if (!File.Exists(logPath)) return;

        string[] lines = File.ReadAllLines(logPath);
        for (int i = lines.Length - 1; i >= 0; i--)
        {
            GameObject entry = Instantiate(reportEntryPrefab, reportContainer);
            TMP_Text txt = entry.GetComponentInChildren<TMP_Text>();
            txt.text = $"Informe {i}: {lines[i]}";
        }
    }

}
