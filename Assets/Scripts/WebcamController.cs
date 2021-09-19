using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class WebcamController : MonoBehaviour
{

#if !UNITY_EDITOR
    private TesseractWrapper _tesseractDriver;
#endif
    public RawImage capturedRawImage;

    public RawImage rawimage;
    WebCamTexture webCamTexture;

    public Text textPrefab;

    public GameObject content;
    
    public ScrollRect scrollRect;

    WebCamDevice[] cam_devices;

    public Toggle toggle;

    void Start()
    {
#if !UNITY_EDITOR
        _tesseractDriver = new TesseractWrapper();
        string datapath = System.IO.Path.Combine(Application.persistentDataPath, "tessdata");
        _tesseractDriver.Init("eng", datapath);
#endif

        cam_devices = WebCamTexture.devices;
        // for debugging purposes, prints available devices to the console
        for (int i = 0; i < cam_devices.Length; i++)
            print("Webcam available: " + cam_devices[i].name);

        
        
            
    }


    //CAMERA 01 SELECT
    public void GoWebCam01()
    {


        webCamTexture = new WebCamTexture(640, 640, 60);
        rawimage.texture = webCamTexture;
        if (webCamTexture != null)
        {
            webCamTexture.Play();
            Debug.Log("Web Cam Connected : " + webCamTexture.deviceName + "\n");
        }




#if UNITY_EDITOR
        rawimage.transform.localEulerAngles = new Vector3(0, 0, 0);
        rawimage.transform.localScale = new Vector3(1, 1, 1);
#else
        rawimage.transform.localEulerAngles = new Vector3(0, 0, 90);
        rawimage.transform.localScale = new Vector3(-1, 1, 1);
#endif
    }



    private void Update()
    {
        if (cam_devices.Length > 0)
        {
            if (!webCamTexture)
                    GoWebCam01();
        }
        else
        {
            cam_devices = WebCamTexture.devices;
        }

    }

    private void AddToTextDisplay(string text, bool isError = false)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        Text t = GameObject.Instantiate(textPrefab, content.transform);

        t.text = text;

        scrollRect.verticalNormalizedPosition = 0f;
    }



    Texture2D RotateTexture(Texture2D originalTexture, bool clockwise)
    {
#if UNITY_EDITOR
      return originalTexture;
#else
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;

#endif


    }

    private Texture2D GetSquareCenteredTexture(Texture2D sourceTexture)
    {
        int squareSize;
        int xPos = 0;
        int yPos = 0;
        
        Color[] c = ((Texture2D)sourceTexture).GetPixels( 0, sourceTexture.height / 2 - 25, sourceTexture.width, 50);// sourceTexture.height );
        Texture2D croppedTexture = new Texture2D(sourceTexture.width, 50); // sourceTexture.height);

        croppedTexture.SetPixels(c);
        croppedTexture.Apply();
        return croppedTexture;
    }


    public void Capture()
    {

        Texture2D workTex = new Texture2D(webCamTexture.width, webCamTexture.height, TextureFormat.ARGB32, false);
        workTex.SetPixels32( 0,
                             0,
                             webCamTexture.width,
                             webCamTexture.height,
                             webCamTexture.GetPixels32());
        workTex.Apply();


        Texture2D rotated = GetSquareCenteredTexture(RotateTexture(workTex, true));

        capturedRawImage.texture = rotated;

#if !UNITY_EDITOR
        AddToTextDisplay(_tesseractDriver.RecognizeFromTexture(rotated, true));// (webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height));;
#endif

        //RecognizeFromTexture(FlipTexture(workTex), true));

        //webCamTexture.GetPixels32(), webCamTexture.width, webCamTexture.height));
        //AddToTextDisplay(_tesseractDriver. RecognizeFromTexture(texture2D, true));

    }

    public void Clear()
    {
        for (int i = 0; i < content.transform.childCount; i++)
            Destroy(content.transform.GetChild(i).gameObject);
    }

    private void SetImageDisplay()
    {
        RectTransform rectTransform = rawimage.GetComponent<RectTransform>();
    /*    rectTransform.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical,
                                                 rectTransform.rect.width * _tesseractDriver.GetHighlightedTexture().height / _tesseractDriver.GetHighlightedTexture().width);*/
//        rawimage.texture = _tesseractDriver.GetHighlightedTexture();
    }

}
