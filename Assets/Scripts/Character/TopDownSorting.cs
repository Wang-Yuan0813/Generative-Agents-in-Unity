using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TopDownSorting : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    private SpriteRenderer spriteRenderer;

    //==================================================
    // SETTINGS
    //==================================================

    [Header("Sorting")]

    [SerializeField]
    private int sortingOffset = 0;

    [SerializeField]
    private bool runOnlyPlaying = true;

    //==================================================
    // UNITY
    //==================================================

    private void Awake()
    {
        spriteRenderer =
            GetComponent<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        // editor mode skip
        if (runOnlyPlaying && !Application.isPlaying)
        {
            return;
        }

        UpdateSorting();
    }

    //==================================================
    // SORTING
    //==================================================

    private void UpdateSorting()
    {
        spriteRenderer.sortingOrder =
            Mathf.RoundToInt(
                -transform.position.y * 100)
            + sortingOffset;
    }
}