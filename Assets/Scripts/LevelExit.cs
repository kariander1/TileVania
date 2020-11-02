using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LevelExit : MonoBehaviour
{
    [SerializeField] float exitDelay = 0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            StartCoroutine(ExitWithDelay(exitDelay));
            player.TriggerWin();
        }
            
    }
    IEnumerator ExitWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
}
