using UnityEngine;
using System.Linq;
using UIText = UnityEngine.UI.Text;

public sealed class WebCamSelector : MonoBehaviour
{
    [SerializeField] UIText _label = null;

    string DescribeResolution(Resolution res)
      => $"{res.width}x{res.height}@{res.refreshRateRatio.value}Hz";

    string DescribeResolutions(WebCamDevice dev)
      => string.Join(", ", dev.availableResolutions.Select(res => DescribeResolution(res)));

    string DescribeDevice(WebCamDevice dev)
    {
        var text = $"Device name: {dev.name}\n";

        text += $"  Type: {dev.kind}\n";

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
        _label.text = string.Join("----\n", WebCamTexture.devices.Select(dev => DescribeDevice(dev)));
    }
}
