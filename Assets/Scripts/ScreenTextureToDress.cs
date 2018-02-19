using UnityEngine;
using System.Collections;

public class ScreenTextureToDress : MonoBehaviour {
    //存取模型
    public GameObject dressTop;
    public GameObject dressBottom;

    public GameObject Plane; //存取面片
    public GameObject Scan_Area;

    private int screenWidth;
    private int screenHeight;
    private Texture2D TextureShot;

    private Vector2 PlaneWH; //記錄面片的寬高
    //記錄面片的世界座標
    private Vector3 TopLeft_Pl_W; //左上角座標
    private Vector3 BottomLeft_Pl_W; //左下角座標
    private Vector3 TopRight_Pl_W; //右上角座標
    private Vector3 BottomRight_Pl_W; //右下角座標
	// Use this for initialization
	void Start () {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
        TextureShot = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ScreenShot_Button()
    {
        TextureShot.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
        //第一個0,0: 獲取屏幕像素的起始點
        //width, height: 獲取屏幕像素的範圍
        //第二個0,0: 填充texture2D時的填充起始座標
        TextureShot.Apply();
        //確認之前對texture2d進行的修改

        //獲取面片寬高的一半
        PlaneWH = new Vector2(Plane.GetComponent<MeshFilter>().mesh.bounds.size.x, Plane.GetComponent<MeshFilter>().mesh.bounds.size.z) * 50.0f * 0.5f;
        //Plane.GetComponent<MeshFilter>().mesh.bounds.size.x: 獲取面片x方向的寬度

        //獲取面片的四個點的世界座標
        TopLeft_Pl_W = Plane.transform.parent.position + new Vector3(-PlaneWH.x, 0, PlaneWH.y);
        BottomLeft_Pl_W = Plane.transform.parent.position + new Vector3(-PlaneWH.x, 0, -PlaneWH.y);
        TopRight_Pl_W = Plane.transform.parent.position + new Vector3(PlaneWH.x, 0, PlaneWH.y);
        BottomRight_Pl_W = Plane.transform.parent.position + new Vector3(PlaneWH.x, 0, -PlaneWH.y);


        //將截圖時四個角的世界座標信息傳遞給shader

        dressTop.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1.0f)); //1f表示浮點數
        dressTop.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1.0f));
        dressTop.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1.0f));
        dressTop.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1.0f));


        dressBottom.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(TopLeft_Pl_W.x, TopLeft_Pl_W.y, TopLeft_Pl_W.z, 1.0f)); //1f表示浮點數
        dressBottom.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(BottomLeft_Pl_W.x, BottomLeft_Pl_W.y, BottomLeft_Pl_W.z, 1.0f));
        dressBottom.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(TopRight_Pl_W.x, TopRight_Pl_W.y, TopRight_Pl_W.z, 1.0f));
        dressBottom.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(BottomRight_Pl_W.x, BottomRight_Pl_W.y, BottomRight_Pl_W.z, 1.0f));

        Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false); //獲取截圖時GPU的投影矩陣
        Matrix4x4 V = Camera.main.worldToCameraMatrix; //獲取截圖時世界座標到到相機座標的投影矩陣
        Matrix4x4 VP = P * V;

        dressTop.GetComponent<Renderer>().material.SetMatrix("_VP", VP); //將截圖時的矩陣轉換信息傳遞給shader
        dressBottom.GetComponent<Renderer>().material.SetMatrix("_VP", VP); //將截圖時的矩陣轉換信息傳遞給shader     

        dressTop.GetComponent<Renderer>().material.mainTexture = TextureShot; //將texture2d複製給模型
        dressBottom.GetComponent<Renderer>().material.mainTexture = TextureShot; //將texture2d複製給模型



    }
}
