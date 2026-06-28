using System.Xml.Serialization;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform cameraTarget;
    public Animator animator;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && animator != null)
        {
            Camera.main.transform.position = new Vector3(cameraTarget.position.x, cameraTarget.position.y, Camera.main.transform.position.z);
            Camera.main.orthographicSize = 6;
            if(name == "2" || name == "3")
            {
                Camera.main.orthographicSize = 7;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && animator != null)
        {
            animator.SetTrigger("close");
        }
    }
}