using UnityEngine;

public class ObjectInfo : MonoBehaviour
{
    [SerializeField]
    [TextArea(3, 20)]
    private string information;
    public string getName()
    {
        return name;
    }
    public string getInformation()
    {
        return information;
    }
}
