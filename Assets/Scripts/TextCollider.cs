using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextCollider : MonoBehaviour
{
    [SerializeField] string text;

    TextGuide guideText;
    void Start()
    {
        guideText = FindObjectOfType<TextGuide>();
        if (!guideText)
            Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        guideText.SetText(this.text);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        guideText.HideText();
    }

}
