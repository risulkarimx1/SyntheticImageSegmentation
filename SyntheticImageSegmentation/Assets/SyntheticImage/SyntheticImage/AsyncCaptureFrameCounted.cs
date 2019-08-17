using System;
using UnityEngine;
using Unity.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
#if UNITY_2018_3_OR_NEWER
using UnityEngine.Rendering;

#else
using UnityEngine.Experimental.Rendering;
#endif

public struct AsyncGPURequest
{
	public AsyncGPUReadbackRequest Request;
	public RenderTexture TempRT;
	public int FrameCount;
}

public class AsyncCaptureFrameCounted : MonoBehaviour
{
	private ImageSavingOperation _imageSavingOperation;
	private void Start()
	{
		_imageSavingOperation = GetComponent<ImageSavingOperation>();
	}

	public bool StartCatpure;

	Queue<AsyncGPURequest> _requests = new Queue<AsyncGPURequest>();

	[SerializeField] private Camera _camera;

	void Update()
	{
		while (_requests.Count > 0)
		{
			var request = _requests.Peek();
			var req = request.Request;
			
			if (req.hasError)
			{
				Debug.Log("GPU readback error detected.");
				_requests.Dequeue();
			}
			else if (req.done)
			{
				_imageSavingOperation.SetRgbRT(request.TempRT);
				RenderTexture.ReleaseTemporary(request.TempRT);
				//var outputBuffer = req.GetData<Color32>();
				//SaveBitmap(outputBuffer, _camera.pixelWidth, _camera.pixelHeight);
				_requests.Dequeue();
			}
			else
			{
				break;
			}
		}
	}

	void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, (RenderTexture) null);

		if (StartCatpure == false)
			return;

		var tempRT = RenderTexture.GetTemporary(source.width, source.height, source.depth, GraphicsFormat.R8G8B8A8_SRGB);
		Graphics.Blit(source, tempRT);

		if (_requests.Count < 8)
		{
			_requests.Enqueue(new AsyncGPURequest
			{
				Request = AsyncGPUReadback.Request(tempRT),
				TempRT = tempRT,
				FrameCount =  Time.frameCount
			});
		}
		else
		{
			Debug.Log("Too many requests.");
		}
	}

	private int c = 0;

	void SaveBitmap(NativeArray<Color32> buffer, int width, int height)
	{
		Debug.Log($"Saving bit map");
		var tex = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
		tex.SetPixels32(buffer.ToArray());
		Debug.Log($"Color at 520 {buffer[520]}");
		tex.Apply();

		File.WriteAllBytes($"test{c++}.png", ImageConversion.EncodeToPNG(tex));
		Destroy(tex);
	}
}

