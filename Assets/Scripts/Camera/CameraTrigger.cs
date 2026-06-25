using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform cameraTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("enter area");
        if (other.CompareTag("Player"))
        {
            Camera.main.transform.position = new Vector3(cameraTarget.position.x, cameraTarget.position.y, Camera.main.transform.position.z);
            Camera.main.orthographicSize = 6;
            if(name == "2" || name == "3")
            {
                Camera.main.orthographicSize = 7;
            }
        }
    }
}