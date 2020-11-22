using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Text m_Text;
    public static string playerSelect;
    public static string boardSelect;
    public static string controlMethod;
    private Dropdown playerSelectDropdown;
    private Dropdown boardSelectDropdown;
    private Dropdown controlMethodDropdown;

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



    void Start()
    {

        playerSelect = fetchInitialDropdownValue(playerSelectDropdown, "PlayerSelect");
        boardSelect = fetchInitialDropdownValue(boardSelectDropdown, "BoardSelect");
        controlMethod = fetchInitialDropdownValue(controlMethodDropdown, "ControlMethod");
        //Add listener for when the value of the Dropdown changes, to take action
        /*
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });
        */
    }

    private string fetchInitialDropdownValue(Dropdown dropdown, string name)
    {
        dropdown = GameObject.Find(name).GetComponent<Dropdown>();
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
