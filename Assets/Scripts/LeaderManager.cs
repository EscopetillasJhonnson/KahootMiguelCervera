using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderManager : MonoBehaviour
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

        foreach (var entry in ordered)
        {
            GameObject row = Instantiate(rankingEntryPrefab, rankingContainer);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            texts[0].text = entry.Score.ToString();      // ScoreText
            texts[1].text = entry.Username;              // UsernameText
        }
    }

    ResultList LoadResults(string path)
    {
        if (!System.IO.File.Exists(path)) return new ResultList();

        XmlSerializer serializer = new XmlSerializer(typeof(ResultList));
        using (FileStream stream = new FileStream(path, FileMode.Open))
        {
            return (ResultList)serializer.Deserialize(stream);
        }
    }
}
