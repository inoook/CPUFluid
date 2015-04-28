using UnityEngine;
using System.Collections;

public class ScreenToLocalPosition : MonoBehaviour {

	public Camera cam;
	public MSAFluid fluid;

	public float mouseMoveV = 0.5f;

	private float pmouseX = 0.0f;
	private float pmouseY = 0.0f;

	private bool mouseDown = false;

	public int emitCount = 60;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo)){
				Vector3 hitPos = hitInfo.point;
				Vector3 localHitPos = hitInfo.transform.InverseTransformPoint(hitPos);

				//
				float x = 1.0f - (localHitPos.x + 5.0f) / 10.0f;
				float y = 1.0f - (localHitPos.z + 5.0f) / 10.0f;

				float mouseVelX = x - pmouseX;
				float mouseVelY = y - pmouseY;

				if(!mouseDown){
					mouseVelX = 0;
					mouseVelY = 0;
					mouseDown = true;
				}

				fluid.colorMode(Processing.ColorMode.HSB, 360, 1, 1);
				float hue = ((x + y) * 180 + Time.time) % 360;
				colorP5 drawColor = fluid.colorHue(hue, 1, 1);
				fluid.colorMode(Processing.ColorMode.RGB, 1);
				
				fluid.addForce(x, y, mouseVelX * mouseMoveV, mouseVelY * mouseMoveV, drawColor, (int)(emitCount * Time.deltaTime * 30));

				pmouseX = x;
				pmouseY = y;
			}
		}

		if(Input.GetMouseButtonUp(0)){
			mouseDown = false;
		}
	}
}
