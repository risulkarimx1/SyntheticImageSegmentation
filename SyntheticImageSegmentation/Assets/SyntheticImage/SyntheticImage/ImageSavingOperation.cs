using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSavingOperation : MonoBehaviour
{
	public RenderTexture RgbRenderTexture;
	public RenderTexture SegmentedRenderTexture;

	public MeshRenderer RGBDisplay;
	public MeshRenderer SegmentedDisplay;

	private void Start()
	{
		
		
	}

	public void SetRgbRT(RenderTexture rt)
	{
		RgbRenderTexture = rt;
		RGBDisplay.material.mainTexture = RgbRenderTexture;
	}

	public void SetSegmentedRT(RenderTexture rt)
	{
		SegmentedDisplay.material.mainTexture = rt;
		RenderTexture.ReleaseTemporary(rt);
	}

	
}
