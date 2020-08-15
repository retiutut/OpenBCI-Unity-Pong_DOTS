using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dropdown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public TextMeshProUGUI output;

    public object onValueChanged { get; internal set; }

    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            output.text = "I LOVE MINECRAFT";
        }
        if (val == 1)
        {
            output.text = "I LOVE FORTNITE";
        }
        if (val == 2)
        {
            output.text = "I LOVE CS:GO";
        }
    }
}
