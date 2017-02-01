using UnityEngine;
using System.Collections;

/*
void fadeToColor(GL gl, float r, float g, float b, float speed) {
    gl.glBlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
    gl.glColor4f(r, g, b, speed);
    gl.glBegin(GL.GL_QUADS);
    gl.glVertex2f(0, 0);
    gl.glVertex2f(width, 0);
    gl.glVertex2f(width, height);
    gl.glVertex2f(0, height);
    gl.glEnd();
}
*/

/**
 *  MSAFluidSolver2DをつかったParticleのUpdate処理
 * 漂っているParticleとaddされるParticleの両方を処理
 */
public class P5ParticleSystem : Processing
{
	public MSAFluidSolver2D fluidSolver;
	public P5ParticleSystemRenderer p5ParticleRenderer;
	
	bool renderUsingVA = true;
	
	public int maxParticles = 1000;
	public int curIndex;
	
	[HideInInspector]
	public P5Particle[] p5particles;
	
	public int maxStayParticles = 1000;
	
	public int allParticles;
	
	public int colorBufferNum = 300;
//tmpInputColorBUffer
	//public int stayColorBufferNum = 6000;
	public int stayColorBufferNum = 500;
	public Color tmpInputColor = Color.black;
	public Color stayColor = Color.black;
	private Color t_stayColor = Color.black;
	
	public bool isReborn = true;

	public void Init (MSAFluidSolver2D fluidSolver)
	{
		allParticles = maxParticles + maxStayParticles;
		p5particles = new P5Particle[allParticles];
		
		curIndex = 0;
		
		for (int i = 0; i < maxParticles; i++) { 
			p5particles [i] = new P5Particle ();
			p5particles [i].fluidSolver = fluidSolver;
			p5particles [i].isSpark = true;
		}
		
		// stay particle
		for (int i = maxParticles; i < allParticles; i++) { 
			p5particles [i] = new P5Particle ();
			p5particles [i].fluidSolver = fluidSolver;
			p5particles [i].isSpark = true;
			/*
			p5particles[i].isStay = true;
			float x = Random.Range(0, Setting.SCREEN_W);
			float y = Random.Range(0, Setting.SCREEN_H);
			Color color = stayColor;
			//Color color = Color.cyan;
			p5particles[i].init(x, y, color);
			float t_alpha = Random.Range(0.4f, 0.9f);
			//p5particles[i].mass = 0.4f;
			p5particles[i].mass = Random.Range(0.4f, 0.9f);
			p5particles[i].StartAppearAnime(t_alpha);
			*/
		}

		p5ParticleRenderer.Init (p5particles);
	}

	public void Reset ()
	{
		curIndex = 0;
		for (int i = 0; i < maxParticles; i++) {
			p5particles [i].color = Color.black;
		}
		
		// stayParticles
		for (int i = maxParticles; i < allParticles; i++) { 
			p5particles [i].isStay = true;
			
			float x = Random.Range (0, Setting.SCREEN_W);
			float y = Random.Range (0, Setting.SCREEN_H);
			Color color = stayColor;
			p5particles [i].init (x, y, color);
			float t_alpha = Random.Range (0.4f, 0.9f);
			p5particles [i].mass = Random.Range (0.4f, 0.9f);
			p5particles [i].StartAppearAnime (t_alpha);
		}
		
		tmpInputColor = Color.black;
		stayColor = Color.black;
		t_stayColor = Color.black;
		avgColor = Color.black;
	}

	public void updateAndDraw ()
	{
		// debug: count updateNum
		//int update_num = 0;
		//int update_stay_num = 0;
		
		for (int i = 0; i < maxParticles; i++) {
			if (p5particles [i].alpha > 0) {
				p5particles [i].update ();
				//update_num ++;
			}
		}
		
		// stay particle
		for (int i = maxParticles; i < allParticles; i++) {
			if (p5particles [i].alpha > 0) {
				p5particles [i].color = stayColor;
				p5particles [i].update ();
				//update_stay_num ++;
			}
		}
	}

	
	// with color
	public void addParticles (float x, float y, int count, Color color)
	{
		addParticles (x, y, count, color, 15);
	}

	public void addParticles (float x, float y, int count, Color color, float randomRange)
	{
		if (!isReborn) {
			return;
		}

		for (int i = 0; i < count; i++) {
			//Vector2 range = Random.insideUnitCircle * randomRange;
			Vector2 range = Random.insideUnitCircle * Random.Range (10, randomRange);
			addParticle (x + range.x, y + range.y, color);
			//addParticle(x + random(-randomRange, randomRange), y + random(-randomRange, randomRange), color);
		}
		
		// color
		tmpInputColor = GetBufferColor (colorBufferNum);
	}
	
	public void addParticle (float x, float y, Color color)
	{
		p5particles [curIndex].init (x, y, color);
		float t_alpha = Processing.random (0.4f, 1);
		p5particles [curIndex].mass = Processing.random (0.05f, 1.0f);
		p5particles [curIndex].StartAppearAnime (t_alpha);
		
		curIndex++;
		if (curIndex >= maxParticles)
			curIndex = 0;
	}

	
	public void SetParticlesReborn (bool isReborn_)
	{
		isReborn = isReborn_;
		for (int i = 0; i < p5particles.Length; i++) {
			p5particles [i].isReborn = isReborn_;
		}
	}
	
	//
	private Color avgColor;

	public Color getAvgColor ()
	{
		return avgColor;
	}

	void Update ()
	{
		avgColor = new Color (0, 0, 0, 0);
		for (int i = 0; i < maxParticles; i++) {
			//if(p5particles[i].alpha > 0) {
			avgColor += p5particles [i].color;
			//}
		}
		avgColor = avgColor / maxParticles;
		avgColor.a = 1.0f;
		
		t_stayColor = GetBufferColor (stayColorBufferNum);
		stayColor += (t_stayColor - stayColor) * Time.deltaTime * 0.5f;
		//stayColor = avgColor;
		//stayColor = tmpInputColor;
	}
	
	//
	public Color GetBufferColor (int colorBufferNum)
	{
		Color bufferColor = Color.black;
		for (int i = 0; i < colorBufferNum; i++) {
			int index = curIndex - i;
			if (index < 0) {
				index = p5particles.Length - 1 - i;
			}
			bufferColor += p5particles [index].color;
		}
		bufferColor = bufferColor / colorBufferNum;
		
		return bufferColor;
	}
	
}