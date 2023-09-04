using UnityEngine;
using UnityEngine.UIElements;

public sealed class ClickToHide : MonoBehaviour
{
    void Start()
    {
        var doc = GetComponent<UIDocument>();
        var preview = doc.rootVisualElement.Q("Preview");
        var main = doc.rootVisualElement.Q("Main");
        preview.AddManipulator(new Clickable(e => main.visible = !main.visible));
    }
}
