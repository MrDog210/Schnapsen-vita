using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class fpsCounter : MonoBehaviour
{
    public TMP_Text fps;
    // Start is called before the first frame update
    private void Update()
    {
        int temp = (int)(1f / Time.unscaledDeltaTime);
        fps.text = temp.ToString();
    }
}
