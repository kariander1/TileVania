using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GuideText : MonoBehaviour
{
    [SerializeField] GameObject triggerArea;

    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        this.text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
