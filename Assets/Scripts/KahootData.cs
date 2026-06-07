using System.Collections.Generic;

[System.Serializable]
public class KahootData
{
    public int id;
    public string title;
    public List<Question> questions;
}

[System.Serializable]
public class Question
{
    public string questionText;
    public List<string> answers;
    public int correctIndex;
    public string imageName;
}
