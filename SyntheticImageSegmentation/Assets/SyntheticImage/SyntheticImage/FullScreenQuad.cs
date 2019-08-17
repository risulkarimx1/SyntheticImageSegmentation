using System;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class FullScreenQuad : MonoBehaviour
{
	private ImageSavingOperation _imageSavingOperation;

	//private RenderTexture _outputRenderTexture;
	//public RenderTexture OutputRenderTexture => _outputRenderTexture;

	public Material OutputMaterial { get; set; }

	public bool IsSegmenting { get; set; }

	private void Start()
	{
		var camera = Camera.main;
		IsSegmenting = false;
		//_outputRenderTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 24, GraphicsFormat.R8G8B8A8_UNorm);
		_imageSavingOperation = GetComponent<ImageSavingOperation>();
	}

	void OnPostRender()
	{
		//if (IsSegmenting)
		//{
		var outputRenderTexture = RenderTexture.GetTemporary(1920, 1080, 24, GraphicsFormat.R8G8B8A8_UNorm);
		var previousRenderTarget = RenderTexture.active;
		RenderTexture.active = outputRenderTexture;

		if (!OutputMaterial)
		{
			return;
		}

		GL.PushMatrix();
		OutputMaterial.SetPass(0);
		GL.LoadOrtho();

		GL.Begin(GL.QUADS); // Quad

		GL.TexCoord2(0, 0);
		GL.Vertex3(0f, 0f, 0);

		GL.TexCoord2(0, 1);
		GL.Vertex3(0f, 1f, 0);

		GL.TexCoord2(1, 1);
		GL.Vertex3(1f, 1f, 0);

		GL.TexCoord2(1, 0);
		GL.Vertex3(1f, 0f, 0);

		GL.End();

		GL.PopMatrix();

		_imageSavingOperation.SetSegmentedRT(outputRenderTexture);

		//var texture = new Texture2D(OutputRenderTexture.width, OutputRenderTexture.height, TextureFormat.ARGB32, false);
		//texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
		//texture.Apply();
		//var data = texture.EncodeToTGA();
		//File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, @"Renderx" + (++count) + ".tga"), data);
		//Destroy(texture);

		RenderTexture.active = previousRenderTarget;
		//}



	}
}