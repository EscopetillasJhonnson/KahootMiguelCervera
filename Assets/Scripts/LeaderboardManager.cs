using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    public Transform kahootListContainer;
    public GameObject kahootButtonPrefab;
    public TMP_Text selectedKahootText;

    public Transform rankingContainer;
    public GameObject rankingEntryPrefab;

    [System.Serializable]
    public class ResultEntry
    {
        public string Username;
        public int Score;
    }

    [XmlRoot("Results")]
    public class ResultList
    {
        [XmlElement("Entry")]
        public List<ResultEntry> entries = new List<ResultEntry>();
    }

    void Start()
    {
        string resultsPath = Application.persistentDataPath + "/Results/";
        if (!Directory.Exists(resultsPath)) return;

        string[] files = Directory.GetFiles(resultsPath, "*.xml");

        foreach (string file in files)
        {
            string kahootName = Path.GetFileNameWithoutExtension(file);
            GameObject btn = Instantiate(kahootButtonPrefab, kahootListContainer);
            btn.GetComponentInChildren<TMP_Text>().text = kahootName;

            btn.GetComponent<Button>().onClick.AddListener(() => {
                selectedKahootText.text = kahootName;
                ShowRanking(file);
            });
        }
    }

    void ShowRanking(string filePath)
    {
        ResultList resultList = LoadResults(filePath);
        var ordered = resultList.entries.OrderByDescending(e => e.Score).ToList();

        foreach (Transform child in rankingContainer) Destroy(child.gameObject);

        if (ordered.Count == 0) return;

        foreach (var entry in ordered)
        {
            GameObject row = Instantiate(rankingEntryPrefab, rankingContainer);
            TMP_Text txt = row.GetComponentInChildren<TMP_Text>();
            txt.text = $"{entry.Score} - {entry.Username}";
        }
    }

    ResultList LoadResults(string path)
    {
        if (!File.Exists(path))
        {
            ErrorReporter.Report("Archivo de resultados no encontrado: " + path);
            return new ResultList();
        }

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ResultList));
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                return (ResultList)serializer.Deserialize(stream);
            }
        }
        catch (System.Exception e)
        {
            ErrorReporter.Report("Error al leer resultados desde " + path + ": " + e.Message);
            return new ResultList();
        }
    }
}
