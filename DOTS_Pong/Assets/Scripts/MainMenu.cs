using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu main;
    public Text m_Text;
    public static string playerSelect;
    public static string boardSelect;
    public static string controlMethod;
    private Dropdown playerSelectDropdown;
    private Dropdown boardSelectDropdown;
    private Dropdown controlMethodDropdown;

    public enum ControlMethod { Keyboard, Mouse, Accelerometer, EMG, Focus };

    public void StartGame()
    {
        Debug.Log("Game Started!!!!!!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Awake()
    {
        if (main != null && main != this)
        {
            return;
        }

        main = this;

        Debug.Log("MAIN MENU IS START");
        playerSelectDropdown = GameObject.Find("PlayerSelect").GetComponent<Dropdown>();
        playerSelect = fetchInitialDropdownValue(playerSelectDropdown);

        boardSelectDropdown = GameObject.Find("BoardSelect").GetComponent<Dropdown>();
        boardSelect = fetchInitialDropdownValue(boardSelectDropdown);

        controlMethodDropdown = GameObject.Find("ControlMethod").GetComponent<Dropdown>();
        string[] enumNames = Enum.GetNames(typeof(ControlMethod));
        List<string> names = new List<string>(enumNames);
        controlMethodDropdown.AddOptions(names);
        controlMethod = fetchInitialDropdownValue(controlMethodDropdown);
        //Add listener for when the value of the Dropdown changes, to take action
        /*
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });
        */
    }

    private string fetchInitialDropdownValue(Dropdown dropdown)
    {
        return dropdown.options[dropdown.value].text;
    }

    //Ouput the new value of the Dropdown into Text
    public void playerSelectCallback(Dropdown dropdown)
    {
        playerSelect = dropdown.options[dropdown.value].text;
        m_Text.text = playerSelect;
    }

    public void boardSelectCallback(Dropdown dropdown)
    {
        boardSelect = dropdown.options[dropdown.value].text;
        m_Text.text = boardSelect;
    }

    public void controlMethodCallback(Dropdown dropdown)
    {
        controlMethod = dropdown.options[dropdown.value].text;
        m_Text.text = controlMethod;
    }
}
