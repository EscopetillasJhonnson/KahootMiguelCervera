using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text timerText;
    public Image questionImage;
    public List<Button> answerButtons;

    private KahootData kahoot;
    private int currentQuestionIndex = 0;
    private float timePerQuestion = 25f;
    private float timeRemaining;
    private bool answered = false;

    private int currentScore = 0;
    private float questionStartTime;

    public TMP_Text scoreText;  

    void Start()
    {
        string json = PlayerPrefs.GetString("SelectedKahoot");
        kahoot = JsonUtility.FromJson<KahootData>(json);
        ShowQuestion();
        scoreText.text = "Puntuación: 0";
    }

    private void ShowQuestion()
    {
        questionStartTime = Time.time;

        answered = false;
        timeRemaining = timePerQuestion;

        Question q = kahoot.questions[currentQuestionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Count; i++)
        {
            TMP_Text btnText = answerButtons[i].GetComponentInChildren<TMP_Text>();
            btnText.text = q.answers[i];
            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }

        if (!string.IsNullOrEmpty(q.imageName))
        {
            Sprite img = Resources.Load<Sprite>("QuestionImages/" + q.imageName);
            questionImage.sprite = img;
            questionImage.gameObject.SetActive(img != null);
        }
        else
        {
            questionImage.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(q.imageName))
        {
            Sprite img = Resources.Load<Sprite>("QuestionImages/" + q.imageName);
            if (img == null)
            {
                ErrorReporter.Report("No se encontró la imagen: " + q.imageName + " en la pregunta " + currentQuestionIndex);
            }
            questionImage.sprite = img;
            questionImage.gameObject.SetActive(img != null);
        }
    }

    void Update()
    {
        if (!answered)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = Math.Ceiling(timeRemaining).ToString();

            if (timeRemaining <= 0)
            {
                answered = true;
                ShowCorrectAnswer();
                StartCoroutine(NextQuestionDelay());
            }
        }
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        if (answered) return;
        answered = true;

        Question q = kahoot.questions[currentQuestionIndex];
        bool isCorrect = selectedIndex == q.correctIndex;

        if (isCorrect)
        {
            float elapsed = Time.time - questionStartTime;
            if (elapsed >= 10f)
            {
                currentScore += 50; 
            }
            else
            {
                currentScore += 100; 
            }
            scoreText.text = "Puntuación: " + currentScore;
        }

        if (selectedIndex >= q.answers.Count)
        {
            ErrorReporter.Report("Índice de respuesta fuera de rango en la pregunta " + currentQuestionIndex);
            return;
        }

        ShowCorrectAnswer();

        StartCoroutine(NextQuestionDelay());
    }

    private void ShowCorrectAnswer()
    {
        Question q = kahoot.questions[currentQuestionIndex];
        for (int i = 0; i < answerButtons.Count; i++)
        {
            Color c = (i == q.correctIndex) ? Color.green : Color.red;
            answerButtons[i].GetComponent<Image>().color = c;
        }
    }

    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(2f);
        currentQuestionIndex++;

        if (currentQuestionIndex < kahoot.questions.Count)
        {
            ResetButtonColors();
            ShowQuestion();
        }
        else
        {
            PlayerPrefs.SetInt("FinalScore", currentScore);
            UnityEngine.SceneManagement.SceneManager.LoadScene("ResultsScene");
        }
    }

    void ResetButtonColors()
    {
        foreach (var btn in answerButtons)
        {
            btn.GetComponent<Image>().color = Color.white;
        }
    }

    
}
