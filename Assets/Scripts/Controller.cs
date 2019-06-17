using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Generator gen;
    public SwipeDots swiperScript;

    private void Start()
    {
        gen = GameObject.Find("Board").GetComponent<Generator>();
        swiperScript = GetComponent<SwipeDots>();
    }

    private void OnMouseDown()
    {
        if (gen.currentState == GameState.move)
        {
            swiperScript.startTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gen.swiper = this.GetComponent<SwipeDots>();
        }
    }

    private void OnMouseUp()
    {
        if (gen.currentState == GameState.move)
        {
            swiperScript.finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            swiperScript.CalculateAngel();
        }
    }
}
