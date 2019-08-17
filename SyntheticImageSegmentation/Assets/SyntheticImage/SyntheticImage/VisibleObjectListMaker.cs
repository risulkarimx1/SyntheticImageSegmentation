using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class VisibleObjectListMaker : MonoBehaviour
{
	

	private FullScreenQuad _fullScreenQuad;

	[SerializeField] private Material _objectListMaterial;
	[SerializeField] private Material _segmentedOutputMaterial;
	[SerializeField] private Camera _camera;


	private Dictionary<int, MeshFilter> _renderedObjectDictionary = new Dictionary<int, MeshFilter>();
	private Dictionary<MeshFilter, int> _meshFilterToTagsMap = new Dictionary<MeshFilter, int>();
	private Dictionary<string, int> _tagIndexDictionary = new Dictionary<string, int>();

	private int _objectIdR;

	public GameObject[] SegmentedObjectParents;

	[SerializeField] private TagsColorsLookup _tagsColorsLookup;
	private Texture2D _tagColorTexture;

	// Start is called before the first frame update
	void Awake()
	{
		CreateTagDictionary();

		//var renderers = (Renderer[])FindObjectsOfType(typeof(Renderer));
		//foreach (var renderer in renderers)
		//{
		//	var renderStatus = renderer.gameObject.AddComponent<RenderStatus>();
		//	renderStatus.SetVisibleRendererList(this);
		//}


		foreach (var segmentedObject in SegmentedObjectParents)
		{
			var renderers = segmentedObject.GetComponentsInChildren<Renderer>();
			foreach (var renderer in renderers)
			{
				var renderStatus = renderer.gameObject.AddComponent<RenderStatus>();
				renderStatus.SetVisibleRendererList(this);
			}
		}

		_objectIdR = Shader.PropertyToID("_ObjectIdR");
		Shader.PropertyToID(nameof(VisibleObjectListMaker));
	}


	private void Start()
	{
		_fullScreenQuad = GetComponent<FullScreenQuad>();

		_tagColorTexture = _tagsColorsLookup.GetTexture();
		_segmentedOutputMaterial.SetFloat("_NumberOfSegments", _tagsColorsLookup.TagColorList.Count);
		_segmentedOutputMaterial.SetTexture("_TagLookUp", _tagColorTexture);
		_fullScreenQuad.OutputMaterial = _segmentedOutputMaterial;

		//StartCoroutine(ProcessVisibleObjects());
		
	}


	private void CreateTagDictionary()
	{
		for (var i = 0; i < _tagsColorsLookup.TagColorList.Count; i++)
		{
			var taglist = _tagsColorsLookup.TagColorList[i];
			_tagIndexDictionary.Add(taglist.Tag, i + 1);
		}
	}

	public void AddRenderer(MeshFilter meshFilter)
	{
		var key = meshFilter.gameObject.GetInstanceID();
		if (_renderedObjectDictionary.ContainsKey(key) == false)
		{
			var tag = FindNearestTag(meshFilter);

			if (tag == "Untagged" || _tagIndexDictionary.ContainsKey(tag) == false)
				return;

			_meshFilterToTagsMap.Add(meshFilter, _tagIndexDictionary[tag]);
			_renderedObjectDictionary.Add(key, meshFilter);
		}
	}

	public void RemoveRenderer(MeshFilter meshFilter)
	{
		var key = meshFilter.gameObject.GetInstanceID();
		if (_renderedObjectDictionary.ContainsKey(key))
		{
			_meshFilterToTagsMap.Remove(meshFilter);
			_renderedObjectDictionary.Remove(key);
		}
	}

	void OnPostRender()
	{

	}
	IEnumerator ProcessVisibleObjects()
	{
		//var renderTexture = new RenderTexture(1920, 1080, 8, RenderTextureFormat.R8);
		var renderTexture  = RenderTexture.GetTemporary(1920, 1080, 0, RenderTextureFormat.R8);
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

			foreach (var meshFilter in _meshFilterToTagsMap)
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

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			PrintDictionary();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			StartCoroutine(ProcessVisibleObjects());
		}
	}

	private string FindNearestTag(MeshFilter meshFilter)
	{
		if (meshFilter.CompareTag("Untagged") == false)
			return meshFilter.tag;

		var currentObject = meshFilter.transform.parent;
		while (currentObject.parent != null)
		{
			if (currentObject.CompareTag("Untagged") == false)
				return currentObject.tag;
			currentObject = currentObject.parent;
		}

		return currentObject.tag;
	}

	// TODO: Debug purpose function, Remove Later - Risul
	private void PrintDictionary()
	{
		Debug.Log($"Object in the list");
		foreach (var renderer in _meshFilterToTagsMap)
		{
			Debug.Log($"{renderer.Key.name}  and Tag Key {_meshFilterToTagsMap[renderer.Key]}");
		}

		Debug.Log($"_____________________________________");
	}
}