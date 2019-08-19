using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectListMaker : MonoBehaviour
{
	public GameObject[] SegmentedObjectParents;

	// Dictionary to hold all rendered object in the scene with RenderStatus script attached
	private Dictionary<int, MeshFilter> _renderedObjectDictionary = new Dictionary<int, MeshFilter>();

	// For each MeshFilter to Tag (in number) dictionary
	public Dictionary<MeshFilter, int> MeshFilterToTagsMap { get; } = new Dictionary<MeshFilter, int>();


	[SerializeField] private SegmentedTagsManager _segmentedTagsManager;

	// Tag Color Look up table


	private void Awake()
	{
		foreach (var segmentedObject in SegmentedObjectParents)
		{
			var renderers = segmentedObject.GetComponentsInChildren<Renderer>();
			foreach (var renderer in renderers)
			{
				var renderStatus = renderer.gameObject.AddComponent<RenderStatus>();
				renderStatus.SetVisibleRendererList(this);
			}
		}
	}


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			PrintDictionary();
		}
	}



	public void AddRenderer(MeshFilter meshFilter)
	{
		var key = meshFilter.gameObject.GetInstanceID();
		if (_renderedObjectDictionary.ContainsKey(key) == false)
		{
			var tag = FindNearestTag(meshFilter);

			if (tag == "Untagged" || _segmentedTagsManager.TagToIndexDictionary.ContainsKey(tag) == false)
				return;

			MeshFilterToTagsMap.Add(meshFilter, _segmentedTagsManager.TagToIndexDictionary[tag]);
			_renderedObjectDictionary.Add(key, meshFilter);
		}
	}

	public void RemoveRenderer(MeshFilter meshFilter)
	{
		var key = meshFilter.gameObject.GetInstanceID();
		if (_renderedObjectDictionary.ContainsKey(key))
		{
			MeshFilterToTagsMap.Remove(meshFilter);
			_renderedObjectDictionary.Remove(key);
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

	private void PrintDictionary()
	{
		Debug.Log($"Object in the list");
		foreach (var renderer in MeshFilterToTagsMap)
		{
			Debug.Log($"{renderer.Key.name}  and Tag Key {MeshFilterToTagsMap[renderer.Key]}");
		}

		Debug.Log($"_____________________________________");
	}
}
