using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public static int finalPlayerHealth = 100;
    public static int finalEnemyHealth = 100;
    
    void Update() 
    {
        if (finalPlayerHealth <= 0 || finalEnemyHealth <= 0) {
            LoadEnd();
        }
    }

    public void LoadEnd() 
    {
        StartCoroutine(LoadLevel(2));
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
}
