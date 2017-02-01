using UnityEngine;
using System.Collections;

/***********************************************************************
 
 Copyright (c) 2008, 2009, Memo Akten, www.memo.tv
 *** The Mega Super Awesome Visuals Company ***
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of MSA Visuals nor the names of its contributors 
 *       may be used to endorse or promote products derived from this software
 *       without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS 
 * OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE 
 * OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
 * OF THE POSSIBILITY OF SUCH DAMAGE. 
 *
 * ***********************************************************************/

public class P5Particle
{
	
	public static float V_SMOOTH = 0.25f;
	
	public MSAFluidSolver2D fluidSolver;
	
	float MOMENTUM = 0.5f;
	float FLUID_FORCE = 0.6f;
//0.6f

	public float x, y;
	public float vx, vy;
	//float radius;       // particle's size
	public float alpha;
	public Color color;
	
	public float mass;
	// 小さいほど動きが遅くなる
	
	public bool isStay = false;
	
	public float randomValue;
	
	public bool isAppearAnime = true;
	public bool isReborn = true;
	
	public float alphaDelta = 0.999f;
	
	/*
    private void init(float x, float y) {
        this.x = x;
        this.y = y;
        vx = 0;
        vy = 0;
    }
    */
	public void init (float x_, float y_, Color color_)
	{
		this.x = x_;
		this.y = y_;
		vx = 0;
		vy = 0;
		randomValue = Random.value;
		
		color = color_;
		
		MOMENTUM = 0.4f + Random.value * 0.2f;
		FLUID_FORCE = 0.6f + Random.Range (-1.0f, 1.0f) * 0.2f;
		
		randomV = Random.value * 10.0f;
		sparkSpeed = Random.Range (10, 20);
	}

	float randomV = 0.0f;
	float sparkSpeed = 0.0f;
	public bool isSpark = false;
	
	float t_alpha = 0.0f;
	float _alpha_per = 0.0f;
	float _appearSpeed = 1.5f;

	public void StartAppearAnime (float target_Alpha)
	{
		t_alpha = target_Alpha;
		alpha = 0.001f;
		
		_alpha_per = 0.001f;
		isAppearAnime = true;
		
		//_alpha_per = 1.0f;
		//isAppearAnime = false;
		
		_appearSpeed = isStay ? 0.05f : 2.0f;
	}

	private void updateAppearAnime ()
	{
		if (isAppearAnime) {
			_alpha_per += Time.deltaTime * _appearSpeed;
			if (_alpha_per > 1.0f) {
				isAppearAnime = false;
				_alpha_per = 1.0f;
			}
		}
	}

	public void AppearAnimeEnd ()
	{
		isAppearAnime = false;
		_alpha_per = 1.0f;
		alpha = t_alpha * _alpha_per;
	}

	float boundaryOffset = 50;
	
	public float vMagnitude;
	
	//public float angle = 0.0f;
	//public Vector3 renderV;
	
	public void update ()
	{
		updateAppearAnime ();
		alpha = t_alpha * _alpha_per;
		
		// only update if particle is visible
		if (alpha == 0) {
			return;
		}
		
		// spark
		if (isSpark) {
			//alpha *= (Mathf.Sin(randomV + Time.realtimeSinceStartup * (20.0f * 1.0f/mass)) + 1.0f) / 2.0f + 0.4f;
			//alpha *= (Mathf.Sin((randomV + Time.realtimeSinceStartup) * 20.0f * (mass * mass)) + 1.0f) / 2.0f + 0.4f;
			alpha *= (Mathf.Sin ((randomV + Time.realtimeSinceStartup) * 10.0f * 1.0f / (mass * mass)) + 1.0f) / 2.0f + 0.4f;
		}
		
		if (fluidSolver == null) {
			return;
		}
		// read fluid info and add to velocity
		int fluidIndex = fluidSolver.getIndexForNormalizedPosition (x * MSAFluid.invWidth, y * MSAFluid.invHeight);
		/*
		vx = fluidSolver.u[fluidIndex] * height * mass * FLUID_FORCE + vx * MOMENTUM;
		vy = fluidSolver.v[fluidIndex] * height * mass * FLUID_FORCE + vy * MOMENTUM;
		*/
		float t_vx = fluidSolver.u [fluidIndex] * height * mass * FLUID_FORCE + vx * MOMENTUM;
		float t_vy = fluidSolver.v [fluidIndex] * height * mass * FLUID_FORCE + vy * MOMENTUM;
		
		//vx += (t_vx - vx)*V_SMOOTH;
		//vy += (t_vy - vy)*V_SMOOTH;
		
		vx = t_vx;
		vy = t_vy;
		
		vMagnitude = (new Vector3 (vx, vy, 0)).magnitude;
		
		/*
		Vector3 v = new Vector3(vx, vy, 0);
		float tt_angle = Vector3.Angle(new Vector3(1,0,0), v);
		angle += (tt_angle - angle)*0.025f;
		renderV = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0)*vMagnitude;
		*/
		
		//if(Mathf.Abs(vx) < 0.075f && Mathf.Abs(vy) < 0.075f){
		if (vMagnitude < P5ParticleSystemRenderer.MIN_MAGNITUDE) {
			/*
			// この処理は無くしたほうがいい？
			// TODO: あったほうがいいけれど、タイトルのとき消えてしまうので処理を再考する
			if(!isAppearAnime){
		        _alpha_per *= alphaDelta;
		        if(_alpha_per < 0.05f) _alpha_per = 0;
			}
			*/
			//vx = 0;
			//vy = 0;
			return;
		}
		/*
		vx += ((fluidSolver.u[fluidIndex] * height * mass * FLUID_FORCE + vx * MOMENTUM) - vx) * 0.1f;
		vy += ((fluidSolver.v[fluidIndex] * height * mass * FLUID_FORCE + vy * MOMENTUM) - vy) * 0.1f;
		*/
		// update position
		x += vx;
		y += vy;
		
		/*
        // bounce of edges
        if(x<0-boundaryOffset) {
            x = 0-boundaryOffset;
            vx *= -1;
        }
        else if(x > width+boundaryOffset) {
            x = width+boundaryOffset;
            vx *= -1;
        }

        if(y<0-boundaryOffset) {
            y = 0-boundaryOffset;
            vy *= -1;
        }
        else if(y > height+boundaryOffset) {
            y = height+boundaryOffset;
            vy *= -1;
        }  
		*/
		
		/*
		// hackish way to make particles glitter when the slow down a lot
		if(vx * vx + vy * vy < 0.001f) {
		    vx = Random.Range(-1, 1) * 0.0025f;
		    vy = Random.Range(-1, 1) * 0.0025f;
		}
		*/
		
		
		// fade out a bit (and kill if alpha == 0);
		if (!isStay) {
			if (x < 0 - boundaryOffset) {
				//x = width+boundaryOffset;
				//vx *= 0.5f;
				
				_alpha_per = 0;
			} else if (x > width + boundaryOffset) {
				//x = 0-boundaryOffset;
				//vx *= 0.5f;
				
				_alpha_per = 0;
			}
	
			if (y < 0 - boundaryOffset) {
				//y = height+boundaryOffset;
				//vy *= 0.5f;
				
				_alpha_per = 0;
			} else if (y > height + boundaryOffset) {
				//y = 0-boundaryOffset;
				//vy *= 0.5f;
				
				_alpha_per = 0;
			}
			
			if (!isAppearAnime) {
				_alpha_per *= alphaDelta;
				if (_alpha_per < 0.05f)
					_alpha_per = 0;
			}
		} else {
			// stay
			if (x < 0 - boundaryOffset || x > width + boundaryOffset) {
				reset ();
			}
			if (y < 0 - boundaryOffset || y > height + boundaryOffset) {
				reset ();
			}
			
			if (!isAppearAnime) {
				_alpha_per *= 0.99f + 0.009f * randomValue;
				if (_alpha_per < 0.05f) {
					reset ();
				}
			}
		}
		
	}

	private void reset ()
	{
		if (!isReborn) {
			// 非表示のまま処理の対象外となる
			_alpha_per = 0;
		} else {
			// reborn  新たな位置に出現
			vx = 0;
			vy = 0;
			//float t_alpha = Random.Range(0.1f, 1.0f);
			float t_alpha = Random.Range (0.5f, 1.0f);
			x = Random.Range (0 - boundaryOffset, width + boundaryOffset);
			y = Random.Range (0 - boundaryOffset, height + boundaryOffset);
			
			StartAppearAnime (t_alpha);
		}
	}

	
	// processing
	public int width {
		get{ return P5Setting.SCREEN_W; }
	}

	public int height {
		get{ return P5Setting.SCREEN_H; }
	}
	
}
/*
public class FloatBuffer
{
	float[] values;
	public void put(int index, float f)
	{
		values[index] = f;
	}
	
	public void newFloatBuffer()
	{
		
	}
}
*/
