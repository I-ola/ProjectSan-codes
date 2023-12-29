using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHealthUi : MonoBehaviour
{
    public Image backgroundImage;
    public Image foregroundImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealthBarPlayer(float amount)
    {
        float parentWidth = GetComponent<RectTransform>().rect.width;
        float width = parentWidth * amount;
        foregroundImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }
}
