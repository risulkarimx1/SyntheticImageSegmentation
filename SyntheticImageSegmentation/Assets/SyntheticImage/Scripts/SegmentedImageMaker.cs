using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class SegmentedImageMaker : MonoBehaviour
{

	[SerializeField] private Material _objectListMaterial; // Material to contain unique color (R value) per segmented object
	[SerializeField] private Material _segmentedOutputMaterial; // Material to hold the Segmented output texture
	[SerializeField] private Camera _camera;

	[SerializeField] private SegmentedTagsManager _segmentedTagsManager;
	
	[SerializeField] private ObjectListMaker _objectListMaker;
	// Shader ID to write data for ObjectListShader
	private int _objectIdR;

	private void Awake()
	{
		_objectIdR = Shader.PropertyToID("_ObjectIdR");
		Shader.PropertyToID(nameof(SegmentedImageMaker));

		_segmentedOutputMaterial.SetTexture("_TagLookUp", _segmentedTagsManager.GetTexture());
		_segmentedOutputMaterial.SetFloat("_NumberOfSegments", _segmentedTagsManager.GetTagsCount());
	}

	private void Start()
	{
		StartCoroutine(ProcessVisibleObjects());
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			StartCoroutine(ProcessVisibleObjects());
		}
	}


	IEnumerator ProcessVisibleObjects()
	{
		//var renderTexture = new RenderTexture(1920, 1080, 8, RenderTextureFormat.R8);
		var renderTexture = RenderTexture.GetTemporary(1920, 1080, 0, RenderTextureFormat.R8);
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.useMipMap = false;
		var renderTargetIdentifier = new RenderTargetIdentifier(renderTexture);

		_segmentedOutputMaterial.SetTexture("_IndexedTexture", renderTexture);

		var commandBuffer = new CommandBuffer();
		commandBuffer.name = "Visible Object Indexing Command Buffer";
		_camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);

		while (true)
		{
			yield return new WaitForEndOfFrame();

			commandBuffer.Clear();
			commandBuffer.SetRenderTarget(renderTargetIdentifier);
			commandBuffer.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));

			foreach (var meshFilter in _objectListMaker.MeshFilterToTagsMap)
			{
				var objectId = meshFilter.Value;
				commandBuffer.SetGlobalFloat(_objectIdR, objectId * 1.0f / 255.0f);

				for (int i = 0; i < meshFilter.Key.mesh.subMeshCount; i++)
				{
					commandBuffer.DrawRenderer(meshFilter.Key.GetComponent<Renderer>(), _objectListMaterial, i);
				}
			}
		}
	}
}
