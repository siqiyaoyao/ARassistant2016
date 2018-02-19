using UnityEngine;
using Vuforia;
using System.Collections;

public class OneRender : MonoBehaviour
{
	public GameObject UIFrame;
	//保存UI扫描框

	public GameObject EarthB2;
	//保存地球仪外框
	public GameObject EarthB3;
	//保存太阳系中的地球

	private Texture2D texture;
	//申请Texture2D变量储存屏幕截图

	private int screenWidth;
	//保存屏幕宽度
	private int screenHeight;
	//保存屏幕高度

	//拾取真正贴图的四个点的坐标
	Vector3 targetAnglePoint1;
	//左上角坐标
	Vector3 targetAnglePoint2;
	//左下角坐标
	Vector3 targetAnglePoint3;
	//右上角坐标
	Vector3 targetAnglePoint4;
	//右下角坐标

	public GameObject plane;
	//储存确定贴图大小的面片物体

	Vector2 halfSize;
	//记录plane宽高的一半值


	void Start()
	{

		screenWidth = Screen.width;
		//屏幕宽
		screenHeight = Screen.height;
		//屏幕高


		texture = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, true);
		//实例化空纹理

		//ScreenShot();
		//执行截图函数


	}

	//截屏函数
	public void ScreenShot()
	{
		//StartCoroutine ("SuccessUI");
		//显示识别成功的提示UI
		UIFrame.SetActive (false);
		//成功读取贴图后将扫描框取消

		texture.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
		//读取屏幕像素信息
		texture.Apply();
		//存储为纹理数据

		halfSize = new Vector2(plane.GetComponent<MeshFilter>().mesh.bounds.size.x, plane.GetComponent<MeshFilter>().mesh.bounds.size.z) * 50.0f*0.5f;
		//获取Plane的长宽的一半值

		//确定真实贴图的世界坐标
		targetAnglePoint1 = transform.parent.position + new Vector3(-halfSize.x, 0, halfSize.y);
		targetAnglePoint2 = transform.parent.position + new Vector3(-halfSize.x, 0, -halfSize.y);
		targetAnglePoint3 = transform.parent.position + new Vector3(halfSize.x, 0, halfSize.y);
		targetAnglePoint4 = transform.parent.position + new Vector3(halfSize.x, 0, -halfSize.y);

		//获取VP值
		Matrix4x4 P = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, false);
		Matrix4x4 V = Camera.main.worldToCameraMatrix;
		Matrix4x4 VP = P * V;

		//给地球的Shader传递贴图四个点的世界坐标，VP，以及贴图
		GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
		GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
		GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
		GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
		GetComponent<Renderer>().material.SetMatrix("_VP", VP);
		GetComponent<Renderer>().material.mainTexture = texture;


		EarthB2.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
		EarthB2.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
		EarthB2.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
		EarthB2.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
		EarthB2.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
		EarthB2.GetComponent<Renderer>().material.mainTexture = texture;

		EarthB3.GetComponent<Renderer>().material.SetVector("_Uvpoint1", new Vector4(targetAnglePoint1.x, targetAnglePoint1.y, targetAnglePoint1.z, 1f));
		EarthB3.GetComponent<Renderer>().material.SetVector("_Uvpoint2", new Vector4(targetAnglePoint2.x, targetAnglePoint2.y, targetAnglePoint2.z, 1f));
		EarthB3.GetComponent<Renderer>().material.SetVector("_Uvpoint3", new Vector4(targetAnglePoint3.x, targetAnglePoint3.y, targetAnglePoint3.z, 1f));
		EarthB3.GetComponent<Renderer>().material.SetVector("_Uvpoint4", new Vector4(targetAnglePoint4.x, targetAnglePoint4.y, targetAnglePoint4.z, 1f));
		EarthB3.GetComponent<Renderer>().material.SetMatrix("_VP", VP);
		EarthB3.GetComponent<Renderer>().material.mainTexture = texture;

		plane.SetActive (false);

	}




}
