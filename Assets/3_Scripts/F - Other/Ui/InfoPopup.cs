using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoPopup : MonoBehaviour
{
    public TextMeshProUGUI text_title;
    public TextMeshProUGUI text_content;

    public void Setup(string title, string content)
    {
        text_title.SetText(title);
        text_content.SetText(content);
    }

    // called by button
    public void Hide()
    {
        Destroy(gameObject);
    }
}
