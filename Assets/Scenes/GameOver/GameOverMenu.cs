using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public void PlayGame()
    {
        //LoadNextLevel();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

