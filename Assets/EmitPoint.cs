using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitPoint : MonoBehaviour {

	public MSAFluid fluid;

	public int emitCount = 10;
	public Color drawColor = Color.white;

	public float speed = 0.2f;

	public float x = 0;
	public float y = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// 1000, 800 protScreenSize
		Vector3 localPos = this.transform.localPosition;
		float x = ((localPos.x / (1000/2.0f)+1.0f)/2.0f);
		float y = ((localPos.y / (800/2.0f)+1.0f)/2.0f);
		
		// x:0 y:0 min
		// x:1 y:1 max
		// x:0.5 y: 0.5 center
		fluid.addForce(x, y, (Random.value-0.5f)* speed, (Random.value-0.5f)* speed, drawColor, emitCount);
	}
}
