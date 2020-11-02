using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float defaultOrtographicView = 7f;
    [SerializeField] float minOrtoView = 1f;
    [SerializeField] float maxOrtoView = 15f;
    [SerializeField] float incrementsStepSize = 0.05f;
    private float currentOrtographicView;
    void Start()
    {
        this.currentOrtographicView = defaultOrtographicView;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Equals))
        {
            currentOrtographicView -= incrementsStepSize;
        }
        else if(Input.GetKey(KeyCode.Minus))
        {
            currentOrtographicView += incrementsStepSize;
        }
        currentOrtographicView = Mathf.Clamp(currentOrtographicView, minOrtoView, maxOrtoView);
        GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.OrthographicSize = currentOrtographicView;
    }
}
