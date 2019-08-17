using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tags Color Table", menuName = "AAI Segmentation/Tag Color Table", order = 1)]
public class TagsColorsLookup : ScriptableObject
{
	public List<TagColor> TagColorList;

	public Texture2D GetTexture()
	{
		var texture = new Texture2D(TagColorList.Count, 1, TextureFormat.ARGB32, false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Repeat
		};

		//texture.SetPixel(0, 0, new Color(0, 0, 0, 0));
		for (var i = 0; i < TagColorList.Count; i++)
			texture.SetPixel(i, 0, TagColorList[i].Color);

		texture.Apply(false);
		return texture;
	}
}