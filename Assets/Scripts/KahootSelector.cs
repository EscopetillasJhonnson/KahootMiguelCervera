using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KahootSelector : MonoBehaviour
{
    public Transform buttonContainer;
    public GameObject buttonPrefab;
    public TMP_InputField usernameInput;

    private string persistentPath => Application.persistentDataPath + "/Kahoots/";

    public void RefreshKahootList()
    {
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        LoadKahootsFromResources();
        LoadKahootsFromPersistentData();
    }

    void Start()
    {
        RefreshKahootList();
    }

    void LoadKahootsFromPersistentData()
    {
        string kahootPath = Application.persistentDataPath + "/Kahoots/";

        if (!Directory.Exists(kahootPath))
        {
            Directory.CreateDirectory(kahootPath);
        }

        foreach (string file in Directory.GetFiles(kahootPath, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(file);

                if (string.IsNullOrEmpty(json))
                {
                    Debug.LogError("Archivo JSON vacío: " + file);
                    continue;
                }

                KahootData kahoot = JsonUtility.FromJson<KahootData>(json);

                if (kahoot == null || string.IsNullOrEmpty(kahoot.title))
                {
                    Debug.LogError("JSON inválido o sin título: " + file);
                    continue;
                }

                CreateButton(kahoot.title, json);
            }
            catch (Exception e)
            {
                Debug.LogError("JSON corrupto ignorado: " + file + " - " + e.Message);
            }
        }
    }

    void LoadKahootsFromResources()
    {
        TextAsset[] jsonFiles = Resources.LoadAll<TextAsset>("KahootsDefault");

        foreach (TextAsset jsonFile in jsonFiles)
        {
            try
            {
                string json = jsonFile.text;
                KahootData kahoot = JsonUtility.FromJson<KahootData>(json);

                if (kahoot == null || string.IsNullOrEmpty(kahoot.title))
                {
                    Debug.LogError("Kahoot por defecto inválido: " + jsonFile.name);
                    continue;
                }

                CreateButton(kahoot.title, json);
            }
            catch (Exception e)
            {
                Debug.LogError("Error al cargar Kahoot por defecto: " + jsonFile.name + " - " + e.Message);
            }
        }
    }

    void CreateButton(string title, string jsonContent)
    {
        GameObject btn = Instantiate(buttonPrefab, buttonContainer);
        btn.GetComponentInChildren<TMP_Text>().text = title;
        btn.GetComponent<Button>().onClick.AddListener(() => OnKahootSelected(jsonContent));
    }

    void OnKahootSelected(string jsonContent)
    {
        string username = usernameInput.text;

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("Usuario no introdujo nombre en el selector de Kahoot");
            username = "#####";
        }

        PlayerPrefs.SetString("Username", username);
        PlayerPrefs.SetString("SelectedKahoot", jsonContent);
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}