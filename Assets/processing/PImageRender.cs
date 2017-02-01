using UnityEngine;
using System.Collections;

public class PImageRender : MonoBehaviour {
	
	public PImage _image;

	public PImage image
	{
		set{ 
			_image = value;
			this.GetComponent<Renderer>().material.mainTexture = _image.texture;
		}
		get{ 
			return _image;
		}
	}

	
	public float w;
	public float h;
	// Update is called once per frame
	void Update () {
		if (_image != null) {
			w = _image.texture.width;
			h = _image.texture.height;
		}
	}
}
