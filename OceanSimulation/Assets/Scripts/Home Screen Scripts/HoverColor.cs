using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Buttons in this version of unity are bugged so that their selected colors are permanent,
/// causing them to display the wrong color even when they are not  selected. This class deals
/// with that issue by working with the built-in event system to manually change the button's
/// color once the user hovers onto or away from it.
/// </summary>
public class HoverColor : MonoBehaviour
{
    public Button button;
    public Color wantedColor;

    private Color originalColor;
    private ColorBlock cb;

    // Start is called before the first frame update
    void Start()
    {
        cb = button.colors;
        originalColor = cb.selectedColor;
    }

    /// <summary>
    /// Changes the color of the button to the desired hover color when called.
    /// </summary>
    public void changeWhenHover()
    {
        cb.selectedColor = wantedColor;
        button.colors = cb;
    }

    /// <summary>
    /// Changes the color of the button to its original (normal) color when called.
    /// </summary>
    public void changeWhenLeaves()
    {
        cb.selectedColor = originalColor;
        button.colors = cb;
    }
}
