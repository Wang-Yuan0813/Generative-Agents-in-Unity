using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text title;
    public TMP_Text information;
    public void Show(string _title, string _information, Vector3 mousePos)
    {
        panel.SetActive(true);
        title.text = _title;
        information.text = _information;
        panel.transform.position = mousePos;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
