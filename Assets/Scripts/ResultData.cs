using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

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