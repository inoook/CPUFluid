using UnityEngine;
using System.Collections;

/**
 *  P5Particle を UnityのParticleSystemでレンダリングする
 */
public class P5ParticleSystemRenderer : MonoBehaviour {
	
	public static float UPSCALE_SIZE = 1.0f;
	public static float UPSCALE_TARGET_SIZE = 3.0f;
	public static float PARTICLE_SIZE = 4.0f;
	public static float PARTICLE_STAY_SIZE = 3.0f;
	public static float PARTICLE_MINMUM_SIZE = 0.025f;//動いていないときのサイズ
	
	public static float MIN_MAGNITUDE = 0.005f;//動いていないと判断するスピード
	
	public UnityEngine.ParticleSystem psys;
	private UnityEngine.ParticleSystem.Particle[] renderParticles;
	public Material mat;

	int maxParticles;
	P5Particle[] particles;
	
	public void Init(P5Particle[] p5particles)
	{
		particles = p5particles;
		maxParticles = particles.Length;
		
		if(psys == null){
			psys = this.gameObject.AddComponent<UnityEngine.ParticleSystem>() as UnityEngine.ParticleSystem;
		}
		psys.GetComponent<Renderer>().material = mat;
		psys.playOnAwake = false;
		psys.enableEmission = false;
		psys.loop = false;
		psys.Stop();
		
		
		renderParticles = new UnityEngine.ParticleSystem.Particle[maxParticles];
		for(int i = 0; i < maxParticles; i++){
			renderParticles[i] = new UnityEngine.ParticleSystem.Particle();
		}
		Reset();
	}
	
	public void Reset()
	{
		for(int i = 0; i < maxParticles; i++){
			renderParticles[i].color = Color.black;
			renderParticles[i].size = PARTICLE_SIZE;
			renderParticles[i].velocity = Vector3.zero;
		}
	}

	void LateUpdate () {
		
		// update all Position & render
		Vector3 offset = new Vector3(Setting.SCREEN_W/2, Setting.SCREEN_H/2, 0);
		Vector3 minSize = new Vector3(1,1,0) * PARTICLE_MINMUM_SIZE;
		
		for(int i = 0; i < maxParticles; i++){
			UnityEngine.ParticleSystem.Particle particle = renderParticles[i];
			P5Particle pObj = particles[i];// processing
			
			float pSize = pObj.isStay ? PARTICLE_STAY_SIZE : PARTICLE_SIZE;

			// StretchedBillboardを使っているのでvelocityが0になると表示されなくなるのを回避
			Vector3 t_velocity = new Vector3(pObj.vx, pObj.vy, 0);
			if(pObj.vMagnitude < MIN_MAGNITUDE){
				t_velocity = minSize;
			}

			float z = -0.5f * pObj.vMagnitude;
			particle.position = new Vector3(pObj.x, pObj.y, z) - offset;
	
			//Stretched Billboardを使用しているときに上部MIN_MAGNITUDEの処理をするときに、
			//レンダリング位置がずれるのを回避する為にスムージング処理。
			//PARTICLE_MINMUM_SIZEを小さくする必要もある。
			
			float smooth = pObj.isStay ? 0.0025f : 0.5f;
			particle.velocity += (t_velocity - particle.velocity)*smooth;
			
			//renderParticle.velocity = t_velocity;
			//renderParticle.velocity = pObj.renderV;
			particle.size = pSize * pObj.mass * UPSCALE_SIZE;
			
			
			Color color = pObj.color;
			color.a = pObj.alpha;
			particle.color = color;

			renderParticles[i] = particle;
		}
		//
		psys.SetParticles(renderParticles, maxParticles);
	}
	
	public void ForceUpdate()
	{
		LateUpdate();
	}
}
