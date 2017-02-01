using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmitCircle : MonoBehaviour {

	public MSAFluid fluid;

	public int emitCount = 10;
	public Color drawColor = Color.white;

	public float force = 0.2f;
	public float forceRange = 0.2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 localPos = this.transform.localPosition;
		float x = ((localPos.x / (1000/2.0f)+1.0f)/2.0f);
		float y = ((localPos.y / (800/2.0f)+1.0f)/2.0f);

		fluid.addForceCircle (x, y, force, drawColor, emitCount, forceRange);
	}
}
