using UnityEngine;
using Michsky.MUIP;

public class SidebarActionButton : MonoBehaviour
{
    [SerializeField] private ButtonManager myButton;
    [SerializeField] private Sprite buttonIcon;

    void Start()
    {
        // Updating button content
        myButton.SetText("New Text");
        myButton.SetIcon(buttonIcon);

        // Set button interactability
        myButton.Interactable(false);
        myButton.Interactable(true);

        // Enable or disable options
        myButton.useRipple = true;
        myButton.enableButtonSounds = true;
        myButton.useClickSound = true;
        myButton.useHoverSound = true;
        myButton.useCustomContent = false;

        // Add events
        myButton.onClick.AddListener(TestFunction);
        myButton.onDoubleClick.AddListener(TestFunction);
        myButton.onHover.AddListener(TestFunction);
        myButton.onLeave.AddListener(TestFunction);
    }

    void TestFunction()
    {
        Debug.Log("Event test");
    }
}