using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void LoadFirstLevel()
    {
        LevelExit exit = FindObjectOfType<LevelExit>();
        exit.gameObject.SetActive(true);

        TextCollider textCollider = FindObjectOfType<TextCollider>();
        textCollider.gameObject.GetComponent<BoxCollider2D>().enabled = true ;

        Destroy(gameObject);
    }
}
