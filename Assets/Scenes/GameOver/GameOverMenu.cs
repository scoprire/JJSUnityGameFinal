using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class GameOverMenu : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public TextMeshProUGUI GameOverText = null;
    
    void Start() {
        Debug.Log(BoardManagerTest.instance.currentPlayerHealth);
        if (GameOverText != null) {
            GameOverText.text = BoardManagerTest.instance.currentPlayerHealth.ToString();
            Debug.Log("hello");
        }
    }

    public void LoadStartLevel() 
    {
        StartCoroutine(LoadLevel(0));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //  Play animation
        transition.SetTrigger("Start");
        //  Wait
        yield return new WaitForSeconds(transitionTime);
        //  Load Scene
        SceneManager.LoadScene(levelIndex);

    }


    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}

