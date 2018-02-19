using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Vuforia;

public class ScreenArea : MonoBehaviour {

    public GameObject scanner;
    public GameObject scanArea;

    private CanvasScaler CanS; //UI螢幕自適度的組件
    private float X_Sc; //實際縮放比例

    //掃描框的四個角的世界座標
    private Vector2 TopLeft_ScreenScan;
    private Vector2 BottomLeft_ScreenScan;
    private Vector2 TopRight_ScreenScan;
    private Vector2 BottomRight_ScreenScan;
    //面片的四個角的世界座標
    private Vector3 TopLeft_Plane;
    private Vector3 BottomLeft_Plane;
    private Vector3 TopRight_Plane;
    private Vector3 BottomRight_Plane;

    private Vector2 PlaneWH; //紀錄面片的寬高

    //面片的屏幕座標
    private Vector2 TopLeft_Plane_Screen;
    private Vector2 BottomLeft_Plane_Screen;
    private Vector2 TopRight_Plane_Screen;
    private Vector2 BottomRight_Plane_Screen;

    private bool HasScan = false; //確定是否已經識別過識別圖

    private enum StateMachine { start, cameraAlready, screenShot, done };
    private StateMachine state = StateMachine.start;

    // Use this for initialization
    void Start()
    {
        CanS = GameObject.Find("Canvas").gameObject.GetComponent<CanvasScaler>();
        //获取控制屏幕自适度的组件
        //第一個GameObejct:要找到的gameobject
        //第二個gameobject:找到的Canvus這個gameobject

        X_Sc = Screen.width / CanS.referenceResolution.x;
        //获取实际的缩放比例
        //CanS.referenceResolution:屏幕的解析度(預定義寬度)
    }

    // Update is called once per frame
    void Update()
    {
        if (state == StateMachine.done)
        {
            if (scanner.activeSelf)
                scanner.SetActive(false);
            if (scanArea.activeSelf)
                scanArea.SetActive(false);
            StopAllCoroutines();
            transform.GetComponent<ScreenArea>().enabled = false;
        }
        else
        {
            //須時時檢測座標位置與範圍
            //400*300: 掃描框
            TopLeft_ScreenScan = new Vector2(Screen.width - 400 * X_Sc, Screen.height + 300 * X_Sc) * 0.5f;
            BottomLeft_ScreenScan = new Vector2(Screen.width - 400 * X_Sc, Screen.height - 300 * X_Sc) * 0.5f;
            TopRight_ScreenScan = new Vector2(Screen.width + 400 * X_Sc, Screen.height + 300 * X_Sc) * 0.5f;
            BottomRight_ScreenScan = new Vector2(Screen.width + 400 * X_Sc, Screen.height - 300 * X_Sc) * 0.5f;

            PlaneWH = new Vector2(gameObject.GetComponent<MeshFilter>().mesh.bounds.size.x, gameObject.GetComponent<MeshFilter>().mesh.bounds.size.z) * 50.0f * 0.5f;
            //"*50"是因为开始获取到的长宽是面片本身的长宽，而场景中我们有缩放因素，父级物体(imagetarget)放大了500倍，而面片自身缩小到了0.1，因此获取实际宽高需要再乘以500*0.1 = 50

            //父物件中心點的世界座標 + 偏移量
            TopLeft_Plane = gameObject.transform.parent.position + new Vector3(-PlaneWH.x, 0, PlaneWH.y);
            BottomLeft_Plane = gameObject.transform.parent.position + new Vector3(-PlaneWH.x, 0, -PlaneWH.y);
            TopRight_Plane = gameObject.transform.parent.position + new Vector3(PlaneWH.x, 0, PlaneWH.y);
            BottomRight_Plane = gameObject.transform.parent.position + new Vector3(PlaneWH.x, 0, -PlaneWH.y);

            //獲取面片的屏幕座標
            TopLeft_Plane_Screen = Camera.main.WorldToScreenPoint(TopLeft_Plane);
            BottomLeft_Plane_Screen = Camera.main.WorldToScreenPoint(BottomLeft_Plane);
            TopRight_Plane_Screen = Camera.main.WorldToScreenPoint(TopRight_Plane);
            BottomRight_Plane_Screen = Camera.main.WorldToScreenPoint(BottomRight_Plane);


            //判斷面片是否在掃描框內
            if (TopLeft_Plane_Screen.x > TopLeft_ScreenScan.x && TopLeft_Plane_Screen.y < TopLeft_ScreenScan.y &&
                BottomLeft_Plane_Screen.x > BottomLeft_ScreenScan.x && BottomLeft_Plane_Screen.y > BottomLeft_ScreenScan.y &&
                TopRight_Plane_Screen.x < TopRight_ScreenScan.x && TopRight_Plane_Screen.y < TopRight_ScreenScan.y &&
                BottomRight_Plane_Screen.x < BottomRight_ScreenScan.x && BottomRight_Plane_Screen.y > BottomRight_ScreenScan.y)
            {
                //當面片完全處於掃描框內
                if (HasScan == false)
                {
                    HasScan = true; //已經識別過了
                    if ((transform.parent.name == "TShirtTarget" && transform.parent.GetComponent<DefaultTrackableEventHandler>().t_1Found)
                        || (transform.parent.name == "DressTarget" && transform.parent.GetComponent<DefaultTrackableEventHandler>().t_2Found))
                    StartCoroutine("WaitForCameraAlready"); //調用截圖的延遲函數
                }
            }
            else
            {
                HasScan = false;
                if (state != StateMachine.start)
                    StopAllCoroutines();
            }
        }
        
    }
    IEnumerator WaitForCameraAlready()
    {
        state = StateMachine.cameraAlready;
        scanArea.SetActive(true);
        scanner.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        scanner.SetActive(false);
        scanArea.SetActive(false);

        StartCoroutine("ScreenShot");
    }
    IEnumerator ScreenShot()
    {
        state = StateMachine.screenShot;
        yield return new WaitForEndOfFrame(); //Waits until the end of the frame after all cameras and GUI is rendered, just before displaying the frame on screen.
        //ReadPixels是从系统缓冲区帧里找像素而不是从图框内找。
        //图应该先在Camera渲染完，存进缓冲之后再ReadPixels。

        if (HasScan) //確保識別圖又移出掃描框範圍時，就不執行截圖
        {
            
            if (transform.parent.name == "TShirtTarget" && transform.parent.GetComponent<DefaultTrackableEventHandler>().t_1Found)
            {
                transform.parent.GetComponent<ScreenTextureToTShirt>().ScreenShot_Button(); //調用截圖函式
                state = StateMachine.done;
            }
            else if (transform.parent.name == "DressTarget" && transform.parent.GetComponent<DefaultTrackableEventHandler>().t_2Found)
            {
                transform.parent.GetComponent<ScreenTextureToDress>().ScreenShot_Button();
                state = StateMachine.done;
            }
        }
    }
}
