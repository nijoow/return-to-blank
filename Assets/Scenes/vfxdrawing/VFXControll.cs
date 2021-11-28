using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
public class VFXControll : MonoBehaviour
{
 
    [SerializeField]
    private VisualEffect visualEffect;

    [SerializeField]
    public float drawing;
    [SerializeField]
    public float lifetimeA;
    [SerializeField]
    public float lifetimeB;

    public float randomFloat;
    public float velocity;

    private KinectManager manager;
    public int textureWidth;
    public int textureHeight;
    public RenderTexture resultRenderTexture;
    public Texture2D texture;
    public Image backGround; 
    public ushort[] depthData;
    Color32[] depthToColor;
    Color32[] outputPixels;
    public int minDepthAmount;
    public bool isDrawing = false;
    public bool isChange = false;
    public int MaxDepth = 20000;
    Vector2 positionNow;
    void Start(){
        textureWidth = KinectWrapper.Constants.DepthImageWidth;
		textureHeight = KinectWrapper.Constants.DepthImageHeight;
        texture = new Texture2D(textureWidth,textureHeight);
        depthData = new ushort[textureWidth*textureHeight];
        depthToColor = new Color32[MaxDepth];
        outputPixels = new Color32[textureHeight* textureWidth];
        string text;
       // velocity = 50;
        text = textureWidth.ToString();
        Debug.Log(text);
        text = textureHeight.ToString();
        Debug.Log(text);
    }
    void Update()
    {
        if(manager == null)
		{
			manager = KinectManager.Instance;
		}
        depthData = manager.GetRawDepthMap();
        int depthIndex=0;
        int textureIndex=0;
        minDepthAmount = 12001;
        notDrawingNow();
        for(int j=0;j<textureHeight;j++){
			for(int i=0;i<textureWidth;i++){
                depthIndex = i + j*textureWidth;
                textureIndex = i + (textureHeight-1-j)*textureWidth;
                if (depthData[depthIndex] < 10000){
                    outputPixels[textureIndex] = Color.black;
                }
    			else if( depthData[depthIndex] >= 10000 && depthData[depthIndex] <= 12000){
                    outputPixels[textureIndex] = Color.blue;
                    if(minDepthAmount > depthData[depthIndex]){
                        minDepthAmount = depthData[depthIndex];
                        positionNow = new Vector2(textureWidth-1-i,textureHeight-1-j);
                        
                        velocity = mapping(minDepthAmount,12000,9700,50,120);
                        isDrawingNow();
                    }
                }
               else{
                    outputPixels[textureIndex] = Color.white;
                }
			}
		}

        texture.SetPixels32(outputPixels);
        texture.Apply();
        Graphics.Blit(texture,resultRenderTexture);
        updateDrawing();
        
        visualEffect.SetFloat("Drawing", drawing);
        visualEffect.SetFloat("LifetimeA", lifetimeA);
        visualEffect.SetFloat("LifetimeB", lifetimeB);
        visualEffect.SetVector2("PositionVector2", positionNow);
        visualEffect.SetFloat("Velocity", velocity);
    }
    private void updateDrawing(){
        if(isDrawing == true){
            lifetimeA = 15;
            lifetimeB = 20;
        }
        else{   
            lifetimeA = 0;
            lifetimeB = 3;
           // backGroundChange();
        }
    }
    private void isDrawingNow(){
        //StopCoroutine(Delay());
        isDrawing = true;
        drawing = 500000;
    }
    private void notDrawingNow(){
        //StartCoroutine(Delay());
        if(isDrawing == true){
            isChange = true;
        }else
            isChange = false;

        isDrawing = false;
        drawing = 0;
    }
    private void backGroundChange(){
        if(isChange == true){
            if(backGround.color == Color.white){
                backGround.color = Color.black;
            }
            else if(backGround.color == Color.black){
                backGround.color = Color.white;
            }
           // velocity = Random.Range(2, 7)*10;
        }
    }
    private float mapping(float x, float in_min, float in_max, float out_min, float out_max){
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }
    IEnumerator Delay(){
        yield return new WaitForSeconds(0.5f);
    }
    void OnGUI() {
       //GUI.DrawTexture(new Rect(0,0,textureWidth/4, textureHeight/4), texture);
    }
}
