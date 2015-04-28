using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class MSAFluid : Processing {
	
	public int FLUID_WIDTH = 70;
	
	public static float invWidth, invHeight;    // inverse of screen dimensions
	float aspectRatio, aspectRatio2;
	
	public MSAFluidSolver2D fluidSolver;
	
	public ParticleSystem particleSystem;
	public ParticleSystemRenderer psysRenderer;
	
	public PImage imgFluid;
	public PImageRender pImageRender;
	
	public bool drawFluid = true;
	public bool enableBoundary = false;
	public bool enableMouse = false;
	public float mouseMoveV = 0.5f;
	
	
	public override void setup() {
	
	    invWidth = 1.0f/Setting.SCREEN_W;
	    invHeight = 1.0f/Setting.SCREEN_H;
	    aspectRatio = width * invHeight;
	    aspectRatio2 = aspectRatio * aspectRatio;
		
	
	    // create fluid and set options
	    fluidSolver = new MSAFluidSolver2D((int)(FLUID_WIDTH), (int)(FLUID_WIDTH * height/width));
		Debug.Log(">>"+(int)(FLUID_WIDTH * height/width));
		Debug.Log(">>"+ ((int)(FLUID_WIDTH * height/width)) * (int)FLUID_WIDTH );
		
	    fluidSolver.enableRGB(true).setFadeSpeed(0.003f).setDeltaT(0.5f).setVisc(0.0001f);
		fluidSolver.enableBoundary = enableBoundary;
	
	    // create image to hold fluid picture
	    imgFluid = createImage(fluidSolver.getWidth(), fluidSolver.getHeight(), PImangeFormat.RGB);
		pImageRender.image = imgFluid;
		
	    // create particle system
		if(particleSystem == null){
			particleSystem = this.gameObject.AddComponent<ParticleSystem>();
		}
		particleSystem.ParticleSystemInit(fluidSolver);

	}

	public float inputX;
	public float inputY;
	public Transform inputAreaTrans;

	public override void mouseMoved() {
		if(!enableMouse){ return; }
		
		float invWidth = 1.0f / Screen.width;
		float invHeight = 1.0f / Screen.height;

	    float mouseNormX = mouseX * invWidth;
	    float mouseNormY = mouseY * invHeight;
	    float mouseVelX = (mouseX - pmouseX) * invWidth;
	    float mouseVelY = (mouseY - pmouseY) * invHeight;
		
		if(isAutoChangeColor){
			float x = mouseNormX;
			float y = mouseNormY;
			
	        colorMode(ColorMode.HSB, 360, 1, 1);
	        float hue = ((x + y) * 180 + frameCount) % 360;
			drawColor = colorHue(hue, 1, 1);
	        colorMode(ColorMode.RGB, 1);
		}
		
	    addForce(mouseNormX, mouseNormY, mouseVelX * mouseMoveV, mouseVelY * mouseMoveV, drawColor, 20);
	}
	
	public float drawV = 2;
	
	private bool isUpdateSolver = true;
	private bool isUpdatePsys = true;
	private bool isUpdateDrawUV = true;
	
	public override void draw() {
		
		if(isSpeep){ return; }
		
	    if(isUpdateSolver){
			fluidSolver.update();
		}
		
		if(drawFluid) {
			int num = fluidSolver.getNumCells();
			
			// redraw texture
		    for(int i = 0; i < num; i++) {
		        float d = drawV;
		        imgFluid.pixels[i] = color(fluidSolver.r[i] * d, fluidSolver.g[i] * d, fluidSolver.b[i] * d);
		    }
		    imgFluid.updatePixels(); //  fastblur(imgFluid, 2);
		    image(imgFluid, 0, 0, width, height);
		}
		
		if(isUpdatePsys){
	    	particleSystem.updateAndDraw();
		}
		if(isUpdateDrawUV){
			// draw UV
			DrawUV();
		}
		
		DrawExtra();
	}
	
	//  TODO: fluidSolverの中でも設定しているので整理の必要あり
	public float visc = 0.0001f;//  大きくするとforceFieldの変化がしにくくなる。変化がすぐ終わる
	public float fadeSpeed = 0.003f;
	public float deltaT = 0.5f; // 小さくするとforceFieldの変化がしにくくなる。初期値に戻りにくくなる
	public int solverIterations = 10;
	
	void DrawExtra()
	{
		fluidSolver.setVisc(visc);
		fluidSolver.setFadeSpeed(fadeSpeed);
		fluidSolver.setDeltaT(deltaT);
		fluidSolver.setSolverIterations(solverIterations);
	}
	
	void DrawUV()
	{
		int _NX = fluidSolver._NX;
		int _NY = fluidSolver._NY;
		float[] u = fluidSolver.u;
		float[] v = fluidSolver.v;
		float marginW = (float)Setting.SCREEN_W / (float)(_NX + 2);
		float marginH = (float)Setting.SCREEN_H / (float)(_NY + 2);
		float offsetX = Setting.SCREEN_W / 2 - this.transform.localPosition.x;
		float offsetY = Setting.SCREEN_H / 2 - this.transform.localPosition.y;
		Color lineColor = new Color(1,1,1,0.35f);
		for (int i = 0; i < _NX+2; i++) {
			for (int j = 0; j < _NY+2; j++) {
				int index = fluidSolver.FLUID_IX(i, j);
				Debug.DrawRay(new Vector3(i * marginW - offsetX, j * marginH - offsetY, 0), new Vector3(u[index], v[index], 0)*1000, lineColor);
			}
		}
		//Debug.DrawRay(new Vector3((_NX + 1) * marginW - offsetX, (_NY + 1) * marginH - offsetY, 0), Vector3.right*100, Color.green);
	}
	
	public override void mousePressed() {
		//Debug.Log("mousePressed");
	    //drawFluid ^= true;
	}
	
	public override void keyPressed() {
	    //println(frameRate);
	}
	
	public bool isAutoChangeColor = false;
	public colorP5 drawColor;
	public float colorMult = 5;
	
	public void addForce(float x, float y, float dx, float dy, colorP5 drawColor, int particleNum, float colorMult, float randomRange) {
		float speed = dx * dx  + dy * dy * aspectRatio2;    // balance the x and y components of speed with the screen aspect ratio

	    if(speed > 0) {

	        if(x<0) x = 0; 
	        else if(x>1) x = 1;
	        if(y<0) y = 0; 
	        else if(y>1) y = 1;
	
	        //float colorMult = 5;
	        float velocityMult = 30.0f;
	
	        int index = fluidSolver.getIndexForNormalizedPosition(x, y);
			
			if(particleNum > 0){

		        fluidSolver.rOld[index]  += red(drawColor) * colorMult;
		        fluidSolver.gOld[index]  += green(drawColor) * colorMult;
		        fluidSolver.bOld[index]  += blue(drawColor) * colorMult;
				
		        particleSystem.addParticles(x * width, y * height, particleNum, drawColor.m_color, randomRange);
			}
			
	        fluidSolver.uOld[index] += dx * velocityMult;
	        fluidSolver.vOld[index] += dy * velocityMult;
	    }
	}

	// add force and dye to fluid, and create particles
	public void addForce(float x, float y, float dx, float dy, colorP5 drawColor, int particleNum, float colorMult) {
		addForce(x, y, dx, dy, drawColor, particleNum, colorMult, 15.0f);
	}

	public void addForce(float x, float y, float dx, float dy, colorP5 drawColor, int particleNum) {
		addForce(x, y, dx, dy, drawColor, particleNum, colorMult);
	}
	
	public void addForce(float x, float y, float dx, float dy) {
	    if(isAutoChangeColor){
	        colorMode(ColorMode.HSB, 360, 1, 1);
	        float hue = ((x + y) * 180 + frameCount) % 360;
			drawColor = colorHue(hue, 1, 1);
	        colorMode(ColorMode.RGB, 1);
		}
		addForce(x, y, dx, dy, drawColor, 10);	
	}
	
	public void addColor(float x, float y, colorP5 drawColor, float colorMult) {
	    int index = fluidSolver.getIndexForNormalizedPosition(x, y);
		
        fluidSolver.rOld[index]  += red(drawColor) * colorMult;
        fluidSolver.gOld[index]  += green(drawColor) * colorMult;
        fluidSolver.bOld[index]  += blue(drawColor) * colorMult;
	}
	
	public void addForceCircle(float x, float y, float force, colorP5 drawColor, int particleNum, float forceRange) {
	   	
		float speed = force * force  + force * force * aspectRatio2;
		
	    if(speed > 0) {
			int _NX = fluidSolver._NX;
			int _NY = fluidSolver._NY;
			float[] u = fluidSolver.u;
			float[] v = fluidSolver.v;
			float marginW = 1.0f / (float)(_NX + 2);
			float marginH = 1.0f / (float)(_NY + 2);
			
			float colorMult = 0.1f;
			float velocityMult = 1.5f;
			
			Vector2 forcePt = new Vector2(x, y/aspectRatio);
			for(int i = 2; i < _NX; i++){
				for(int j = 2; j < _NY; j ++){
					int index = fluidSolver.FLUID_IX(i, j);
					
					Vector2 pt = new Vector2(marginW * i, marginH * j/aspectRatio);
					float dist = Vector2.Distance(forcePt, pt );
					if(dist > 0.01f && dist < forceRange){
						Vector2 direction = pt - forcePt;
						direction = direction.normalized;
						
						fluidSolver.uOld[index] += force * direction.x * velocityMult * (1 - dist/forceRange);
		        		fluidSolver.vOld[index] += force * direction.y * velocityMult * (1 - dist/forceRange);
					}else{
						fluidSolver.uOld[index] *= 0.9f;
		        		fluidSolver.vOld[index] *= 0.9f;
					}
				}
			}
			
			if(force > 0){
				int indexColorEmit = fluidSolver.getIndexForNormalizedPosition(x, y);
				fluidSolver.rOld[indexColorEmit]  += red(drawColor) * colorMult;
				fluidSolver.gOld[indexColorEmit]  += green(drawColor) * colorMult;
				fluidSolver.bOld[indexColorEmit]  += blue(drawColor) * colorMult;
				
				float range = Random.Range(5, 10) * 2.0f;// 0-60
				Vector2 randomPos = Random.insideUnitCircle * 0.05f;
				particleSystem.addParticles((x+randomPos.x) * width, (y+randomPos.y) * height, particleNum, drawColor.m_color, range);
			}
			
	    }
	}
	
	public void addForceCircle2(float x, float y, float force, colorP5 drawColor, int particleNum) {
	   	float speed = force * force  + force * force * aspectRatio2;
		
	    if(speed > 0) {
			
			float range = 0.15f;
			float split = 18;
			float distSplit = 2;
			
			float splitRad = Mathf.PI * 2 / split;
			float splitDist = range / distSplit;
			
			//float offsetRad = Random.value * Mathf.PI*2;
			float offsetRad = Time.time * 0.1f;
			for(int i = 0; i < split; i++){
				for(int j = 1; j < distSplit; j ++){
					float rndForce = force * Random.value;
					float rad = splitRad * i + offsetRad;
					
					float dist = splitDist * j;
					float tx = x + dist * Mathf.Cos(rad);
					float ty = y + dist * Mathf.Sin(rad);
				
					float tdx = rndForce * Mathf.Cos(rad);
					float tdy = rndForce * Mathf.Sin(rad)*aspectRatio;
					addForce(tx, ty, tdx, tdy, drawColor, 0);// no Particle emit;
				}
			}
			
			if(force > 0){
				float colorMult = 5;
				int indexColorEmit = fluidSolver.getIndexForNormalizedPosition(x, y);
				fluidSolver.rOld[indexColorEmit]  += red(drawColor) * colorMult;
				fluidSolver.gOld[indexColorEmit]  += green(drawColor) * colorMult;
				fluidSolver.bOld[indexColorEmit]  += blue(drawColor) * colorMult;
				
				particleSystem.addParticles(x * width, y * height, particleNum, drawColor.m_color, 100);
			}
	    }
	}
	
	public bool isSpeep = false;
	
	public void Sleep(bool isSleep_){
		
		if(isSpeep == isSleep_){ return; }
		
		particleSystem.enabled = !isSleep_;
		psysRenderer.enabled = !isSleep_;
		
		if(isSleep_){
			// 前の画面が残るので更新する
			draw();
			psysRenderer.ForceUpdate();
		}
		
		isSpeep = isSleep_;
	}
	
	public void Reset()
	{
		psysRenderer.Reset();
		fluidSolver.reset();
		particleSystem.Reset();
	}
	
	public override void OnGUI()
	{
		base.OnGUI();
	}

	public void SetParticlesReborn(bool enable)
	{
		particleSystem.SetParticlesReborn(enable);
	}
	
}


