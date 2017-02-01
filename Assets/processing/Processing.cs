using UnityEngine;
using System.Collections;


public class Processing : MonoBehaviour
{
	public enum PImangeFormat
	{
		RGB
	}
	
	public static float random(float min, float max){
		return Random.Range(min, max);
	}
	
	// color --------------------------
	
	public enum ColorMode{
		RGB, HSB
	}
	private ColorMode currentColorMode = ColorMode.RGB;
	private float m_range1;
	private float m_range2;
	private float m_range3;
	
	public void colorMode(ColorMode mode)
	{
		currentColorMode = mode;
	}
	public void colorMode(ColorMode mode, float range)
	{
		colorMode(mode);
		m_range1 = m_range2 = m_range3 = range;
	}
	public void colorMode(ColorMode mode, float range1, float range2, float range3)
	{
		colorMode(mode);
		m_range1 = range1;
		m_range2 = range2;
		m_range3 = range3;
	}
	
	/*
	public colorP5 color(int r, int g, int b){
		return new colorP5((float)r/255.0f, (float)g/255.0f, (float)b/255.0f, 1.0f);
	}
	*/
	public colorP5 color(float r, float g, float b){
		return new colorP5(r, g, b, 1.0f);
	}
	
	// hue
	public colorP5 colorHue(float hue, float s, float b){
		return new colorP5(hue, s, b);
	}
	
	public float red(colorP5 color)
	{
		return color.m_color.r;
	}
	public float green(colorP5 color)
	{
		return color.m_color.g;
	}
	public float blue(colorP5 color)
	{
		return color.m_color.b;
	}
	// image  --------------------------
	
	public PImage createImage(int width, int height, PImangeFormat format)
	{
		return new PImage(width, height);
	}
	
	public void image(PImage img, int x, int y, int width, int height)
	{
		/*
		image(画像ファイル名, x, y); – 座標(x, y)を左上にして、画像を表示
		image(画像ファイル名, x, y, width, height); – 座標(x, y)を左上にして、幅(width)と高さ(height)を指定して画像を表示
		*/
	}
	
	// debug  --------------------------
	
	public void println(string str)
	{
		Debug.Log(str);
	}
	
	//
	
	public int width
	{
		get{ return P5Setting.SCREEN_W; }
	}
	public int height
	{
		get{ return P5Setting.SCREEN_H; }
	}
	
	protected int frameCount;
	
	protected float mouseX;
	protected float mouseY;
	protected float pmouseX;
	protected float pmouseY;
	
	void Awake()
	{
		Vector3 mousePos = Input.mousePosition;
		pmouseX = mousePos.x;
		pmouseX = mousePos.y;
		
		frameCount = 0;
		
		setup();
		
	}
	public virtual void Update()
	{
		frameCount ++;
		Vector3 mousePos = Input.mousePosition;
		mouseX = mousePos.x;
		mouseY = mousePos.y;
		
		//
		draw();
		mouseMoved();
		//
		pmouseX = mouseX;
		pmouseY = mouseY;
	}
	
	
	protected string key;
	public virtual void OnGUI()
	{
		Event e = Event.current;
		if (e.isKey){
			key = (e.keyCode).ToString();
		}
		
		if(e.isMouse){
			if(e.type == EventType.MouseDown){
				mousePressed();
			}
			
		}
	}
	
	public virtual void setup(){
		
	}
	
	public virtual void mouseMoved()
	{
	}
	
	public virtual void draw()
	{
	}
	
	public virtual void mousePressed() {
		
	}
	
	public virtual void keyPressed() {
		
	}
	
}

public class PImage
{
	[HideInInspector]
	public Texture2D texture;
	
	[HideInInspector]
	public colorP5[] pixels;
	
	public PImage(int w, int h){
		texture = new Texture2D(w, h, TextureFormat.RGB24, false);
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = FilterMode.Trilinear;
		Color32[] colors = texture.GetPixels32();
		
		pixels = new colorP5[colors.Length];
		
		for(int i = 0; i < colors.Length; i++){
			colors[i] = Color.black;
		}
		texture.SetPixels32(colors, 0);
		texture.Apply( false );
	}
	
	public void updatePixels()
	{
		Color32[] colors = ConvColors32(pixels); 
		texture.SetPixels32(colors, 0);
		texture.Apply( false );
	}
	
	//
	public static Color ConvColor(colorP5 cP5)
	{
		return cP5.m_color;
	}
	
	public static Color[] ConvColors(colorP5[] cP5s)
	{
		Color[] colors = new Color[cP5s.Length];
		for(int i = 0; i < cP5s.Length; i++){
			colors[i] = ConvColor(cP5s[i]);
		}
		return colors;
	}
	
	public static Color32[] ConvColors32(colorP5[] cP5s)
	{
		Color32[] colors = new Color32[cP5s.Length];
		for(int i = 0; i < cP5s.Length; i++){
			colors[i] = (Color32)( cP5s[i].m_color );
		}
		return colors;
	}
}

[System.Serializable]
public class colorP5
{
	public Color m_color;
	
	public colorP5(float r, float g, float b, float a)
	{
		m_color = colorToColor(r, g, b, a);
	}
	
	public colorP5(float h, float s, float b)
	{
		m_color = colorToColor( new HSBColor(h/360.0f, s, b).ToColor() );
	}
	/*
	public colorP5(int r, int g, int b, int a)
	{
		m_color = colorToColor32(r, g, b, a);
	}
	*/
	
	public static Color colorToColor(Color color)
	{
		return color;
	}
	
	public static Color colorToColor(float r, float g, float b, float a)
	{
		return new Color( r, g, b, a );
	}
	
	public static Color32 colorToColor32(Color color)
	{
		return new Color32((byte)((int)(color.r*255.0f)), (byte)((int)(color.g*255.0f)), (byte)((int)(color.b*255.0f)), (byte)((int)(color.a*255.0f)) );
	}
	
	public static Color32 colorToColor32(float r, float g, float b, float a)
	{
		//return new Color32((byte)((int)(r*255.0f)), (byte)((int)(g*255.0f)), (byte)((int)(b*255.0f)), (byte)((int)(a*255.0f)) );
		return new Color32(floatToByte(r), floatToByte(g), floatToByte(b), floatToByte(a) );
	}
	public static byte floatToByte(float v)
	{
		//return (byte)( UnityEngine.Mathf.FloorToInt(v * 255) );
		return (byte)((UnityEngine.Mathf.FloorToInt(v * 255.0f)) & 0xFF);
	}
	
	public static Color ToColor(colorP5 colorP5_)
	{
		return colorP5_.m_color;
	}
	
	public static colorP5 ToColorP5(Color color)
	{
		return new colorP5(color.r, color.g, color.b, color.a);
	}
}