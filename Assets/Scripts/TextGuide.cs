using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TextGuide : MonoBehaviour
{
    [SerializeField] Text guideText;
    [SerializeField] float fadeInterval = 0.1f;

    Color guideTextColor;
    bool fadeIn = false;
    private string currentText="";
    // Start is called before the first frame update
    void Start()
    {

        guideText = GetComponent<Text>();
        guideTextColor = guideText.color;
        
        HideText();
    }

    // Update is called once per frame
    void Update()
    {
        guideText.text = currentText;
        if(fadeIn && guideText.color.a < 1)
        {
            guideTextColor = new Color(guideTextColor.r, guideTextColor.g, guideTextColor.b, guideTextColor.a + fadeInterval);
        }
        else if(!fadeIn && guideText.color.a > 0)
        {
            guideTextColor = new Color(guideTextColor.r, guideTextColor.g, guideTextColor.b, guideTextColor.a - fadeInterval);
        }
        guideText.color = guideTextColor;
    }
    public void HideText()
    {
        fadeIn = false;
        //currentText = "";
    }
    public void SetText(string text)
    {
        fadeIn = true;
        currentText = text;
    }
}
