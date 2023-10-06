using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    void Update()
    {
        LoadNextScene();
    }

    // When "J" is pressed, Scene with next index loads.
    void LoadNextScene()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
