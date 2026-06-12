using UnityEngine;

public class BeginningPanelControl : MonoBehaviour
{
    [SerializeField]
    private GameObject inputFieldObject;

    public RectTransform target;
    public float scaleAmount = 1.2f;
    public float speed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;

    
    void Start()
    {
        originalScale = target.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            target.localScale = originalScale * scaleAmount;
        }
        target.localScale = Vector3.Lerp(target.localScale, originalScale, Time.deltaTime * speed);
    }
    public void DisableAfterAnimation()
    {
        Destroy(gameObject);
        //Destroy(inputFieldObject);
        //inputField.gameObject.;//only input once
    }
}
