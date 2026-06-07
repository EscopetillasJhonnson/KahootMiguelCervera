using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using Directory = System.IO.Directory;

public class ResultsManager : MonoBehaviour
{
    public TMP_Text scoreText;          // Texto dinámico de la puntuación
    public TMP_Text labelText;          // Texto fijo "Has obtenido la siguiente puntuación:"
    public Button leaderboardButton;
    public Button menuButton;
    public Button selectorButton;

    public GameObject namePanel;
    public TMP_InputField nameInputField;
    public Button saveButton;

    private int finalScore;

    void Start()
    {
        finalScore = PlayerPrefs.GetInt("FinalScore", 0);
        labelText.text = "Has obtenido la siguiente puntuación:";
        StartCoroutine(AnimateScore(finalScore, 0.5f));

        leaderboardButton.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("LeaderBoard");
        });
        menuButton.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        });
        selectorButton.onClick.AddListener(() => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("QuizzesSelector");
        });

        // 🔹 Siempre mostrar el panel de nombre al terminar
        namePanel.SetActive(true);
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(SaveName);
    }

    void SaveName()
    {
        string name = nameInputField.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            name = "#####";
        }

        PlayerPrefs.SetString("Username", name);
        namePanel.SetActive(false);

        SaveResult(name, finalScore);
    }

    void SaveResult(string username, int score)
    {
        string resultsPath = Application.persistentDataPath + "/Results/";
        if (!Directory.Exists(resultsPath)) Directory.CreateDirectory(resultsPath);

        string kahootJson = PlayerPrefs.GetString("SelectedKahoot");
        KahootData kahoot = JsonUtility.FromJson<KahootData>(kahootJson);
        string filePath = Path.Combine(resultsPath, kahoot.title + ".xml");

        ResultList resultList;
        if (System.IO.File.Exists(filePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ResultList));
            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                resultList = (ResultList)serializer.Deserialize(stream);
            }
        }
        else
        {
            resultList = new ResultList();
        }

        resultList.entries.Add(new ResultEntry { Username = username, Score = score });

        // Guardar XML
        XmlSerializer serializerSave = new XmlSerializer(typeof(ResultList));
        using (FileStream stream = new FileStream(filePath, FileMode.Create))
        {
            serializerSave.Serialize(stream, resultList);

        }
    }

    IEnumerator AnimateScore(int targetScore, float duration)
    {
        float elapsed = 0f;
        int startScore = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            int currentScore = Mathf.RoundToInt(Mathf.Lerp(startScore, targetScore, t));
            scoreText.text = currentScore.ToString();
            yield return null;
        }

        scoreText.text = targetScore.ToString();
    }

}
