using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public sealed class WebCamDescriptor : MonoBehaviour
{
    string DescribeResolution(Resolution res)
      => $"{res.width}x{res.height}@{res.refreshRateRatio.value}Hz";

    string DescribeResolutions(WebCamDevice dev)
      => string.Join(", ", dev.availableResolutions.Select(res => DescribeResolution(res)));

    string DescribeDevice(WebCamDevice dev)
    {
        var text = $"Device name: {dev.name}\n  Type: {dev.kind}\n";

        if (dev.depthCameraName != null)
            text += $"  Depth support: ({dev.depthCameraName})\n";

        text += $"  Direction: {(dev.isFrontFacing ? "Front" : "Rear")}\n";

        if (dev.isAutoFocusPointSupported)
            text += "  Auto focus support\n";

        if (dev.availableResolutions != null)
            text += $"  Supported resolutions: {DescribeResolutions(dev)}\n";

        return text;
    }

    async Awaitable Start()
    {
        await Application.RequestUserAuthorization(UserAuthorization.WebCam);

        var descs = WebCamTexture.devices.Select(dev => DescribeDevice(dev));

        var doc = GetComponent<UIDocument>();
        var label = (Label)doc.rootVisualElement.Q("Description");
        label.text = string.Join("----\n", descs);
    }
}
