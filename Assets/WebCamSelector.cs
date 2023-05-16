using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public sealed class WebCamSelector : MonoBehaviour
{
    WebCamTexture _webcam;
    RenderTexture _temp;

    Vector3 CalcAspectRatioFixScale()
    {
        var src = (float)_webcam.width / _webcam.height;
        var dst = (float)Screen.width / Screen.height;
        if (dst > src)
            return new Vector3(src / dst, 1, 1);
        else
            return new Vector3(1, dst / src, 1);
    }

    async void SelectDevice(string name)
    {
        if (_webcam != null)
        {
            Destroy(_webcam);
            Destroy(_temp);
        }

        _webcam = new WebCamTexture(name);
        _webcam.Play();

        while (_webcam.width < 32) await Awaitable.NextFrameAsync();

        _temp = new RenderTexture(_webcam.width, _webcam.height, 0);

        var doc = GetComponent<UIDocument>();
        var preview = (VisualElement)doc.rootVisualElement.Q("Preview");
        preview.style.backgroundImage = Background.FromRenderTexture(_temp);
        preview.transform.scale = CalcAspectRatioFixScale();
    }

    async Awaitable Start()
    {
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        var doc = GetComponent<UIDocument>();
        var selector = (DropdownField)doc.rootVisualElement.Q("Selector");

        var c_devs = WebCamTexture.devices.Select(dev => dev.name);
        var d_devs = WebCamTexture.devices.Select(dev => dev.depthCameraName);
        selector.choices = c_devs.Concat(d_devs.Where(s => s != null)).ToList();
        selector.RegisterValueChangedCallback(e => SelectDevice(e.newValue));
    }

    void Update()
    {
        if (_webcam == null) return;
        var vflip = _webcam.videoVerticallyMirrored;
        var scale = new Vector2(1, vflip ? -1 : 1);
        var offset = new Vector2(0, vflip ? 1 : 0);
        Graphics.Blit(_webcam, _temp, scale, offset);
    }
}
