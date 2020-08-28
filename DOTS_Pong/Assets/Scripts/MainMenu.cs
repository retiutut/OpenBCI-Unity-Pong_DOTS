using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Text m_Text;

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
        //Fetch the Dropdown GameObject
       
        //Add listener for when the value of the Dropdown changes, to take action
        /*
        m_Dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(m_Dropdown);
        });
        */
    }

    //Ouput the new value of the Dropdown into Text
    public void DropdownValueChanged(Dropdown dropdown)
    {
        m_Text.text = dropdown.options[dropdown.value].text;
    }

}
