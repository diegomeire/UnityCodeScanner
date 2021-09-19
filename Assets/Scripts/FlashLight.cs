using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class FlashLight : MonoBehaviour
{

#if UNITY_IOS

    [DllImport("__Internal")]
    static extern void _EnableFlashlight(bool enable);

    [DllImport("__Internal")]
    static extern void _SetFlashlightLevel(float val);

    [DllImport("__Internal")]
    static extern bool _DeviceHasFlashlight();

    public bool HasTorch
    {
        get {
            return _DeviceHasFlashlight();
        }
    }

    public void ToggleFlashLight(Toggle toggle)
    {
        EnableFlashlight(toggle.isOn);
    }

    public void TurnOn()
    {
        EnableFlashlight(true);
    }

	public void EnableFlashlight(bool enable)
	{
		_EnableFlashlight(enable);
	}

	public void SetFlashlightIntensity(float intensity)
	{
		intensity = Mathf.Clamp01(intensity);
		_SetFlashlightLevel(intensity);
	}



#elif UNITY_ANDROID

    public AndroidJavaClass javaObject;
    void Start()
    {
        javaObject = new AndroidJavaClass("com.myflashlight.flashlightlib.Flashlight");
    }

    public void TurnOn()
    {
        javaObject.CallStatic("on", GetUnityActivity());
    }
    
    public void TurnOff()
    {
        javaObject.CallStatic("off", GetUnityActivity());
    }

    public void ToggleFlashLight(Toggle toggle)
    {
        if (toggle.isOn)
            TurnOn();
        else
            TurnOff();
        
    }
    
     AndroidJavaObject GetUnityActivity(){
         using ( var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
         {
             return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
         }
     }

#endif
}