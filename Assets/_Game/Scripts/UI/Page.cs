using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    public List<Entry> entries = new List<Entry>();

    public int index;

    private Journal journal;

    private void Start()
    {
        journal = GameObject.Find("Journal Panel").GetComponent<Journal>();
    }
}
