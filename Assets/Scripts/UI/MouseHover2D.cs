using UnityEngine;
public class MouseHover2D : MonoBehaviour
{
    public Camera cam;
    public TooltipUI tooltip;
    void Update()
    {

        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] hits = Physics2D.OverlapPointAll(mousePos);
        foreach (var hit in hits) 
        {
            if (hit != null && hit.CompareTag("HoverObject"))
            {
                string title = "null";
                string information = "null";
                ObjectInfo info = hit.GetComponent<ObjectInfo>();
                if (info != null)
                {
                    title = info.getName();
                    information = info.getInformation();
                }
                tooltip.Show(title, information, Input.mousePosition);
                return;
            }
        }
        tooltip.Hide();
    }
}