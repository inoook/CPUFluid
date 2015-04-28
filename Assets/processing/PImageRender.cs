using UnityEngine;
using System.Collections;

public class PImageRender : MonoBehaviour {
	
	public PImage image;
	// Use this for initialization
	void Start () {
	
		this.GetComponent<Renderer>().material.mainTexture = image.texture;
	}
	
	public float w;
	public float h;
	// Update is called once per frame
	void Update () {
		w = image.texture.width;
		h = image.texture.height;
	}
}
