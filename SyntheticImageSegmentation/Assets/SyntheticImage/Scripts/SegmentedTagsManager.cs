using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentedTagsManager : MonoBehaviour
{
	[SerializeField] private TagsColorsLookup _tagsColorsLookup;

	public Dictionary<string, int> TagToIndexDictionary { get; private set; } = new Dictionary<string, int>();

	// Tag Tag to int number dictionary - assigning an uniqe number per tag

	void Awake()
	{
		CreateTagDictionary();
	}
	private void CreateTagDictionary()
	{
		for (var i = 0; i < _tagsColorsLookup.TagColorList.Count; i++)
		{
			var taglist = _tagsColorsLookup.TagColorList[i];
			TagToIndexDictionary.Add(taglist.Tag, i + 1);
		}
	}
	public Texture2D GetTexture()
	{
		var texture = new Texture2D(_tagsColorsLookup.TagColorList.Count, 1, TextureFormat.ARGB32, false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Repeat
		};

		//texture.SetPixel(0, 0, new Color(0, 0, 0, 0));
		for (var i = 0; i < _tagsColorsLookup.TagColorList.Count; i++)
			texture.SetPixel(i, 0, _tagsColorsLookup.TagColorList[i].Color);

		texture.Apply(false);
		return texture;
	}

	public int GetTagsCount()
	{
		return _tagsColorsLookup.TagColorList.Count;
	}
}
