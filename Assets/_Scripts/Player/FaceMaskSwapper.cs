using UnityEngine;

public class FaceMaskSwapper : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The SpriteRenderer that will display the mask.")]
    [SerializeField] private SpriteRenderer maskRenderer;

    [Tooltip("The list of mask sprites to cycle through. Order matters: 0=Red, 1=White, 2=Black (or however you want to match).")]
    [SerializeField] private Sprite[] maskSprites;

    [Tooltip("Key to trigger the mask swap.")]
    [SerializeField] private KeyCode swapKey = KeyCode.Space;

    private int _currentIndex = 0;

    void Start()
    {
        // Initialize with the first mask if available
        if (maskSprites != null && maskSprites.Length > 0 && maskRenderer != null)
        {
            UpdateMask();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(swapKey))
        {
            CycleNextMask();
        }
    }

    public void CycleNextMask()
    {
        if (maskSprites == null || maskSprites.Length == 0) return;

        _currentIndex = (_currentIndex + 1) % maskSprites.Length;
        UpdateMask();
    }

    private void UpdateMask()
    {
        if (maskRenderer != null && _currentIndex < maskSprites.Length)
        {
            maskRenderer.sprite = maskSprites[_currentIndex];
        }
    }

    // Optional: Public method to set specific mask index (useful if other scripts need to force a mask)
    public void SetMaskIndex(int index)
    {
        if (maskSprites == null || index < 0 || index >= maskSprites.Length) return;
        _currentIndex = index;
        UpdateMask();
    }
}
