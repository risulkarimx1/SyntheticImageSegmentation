using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tags Color Table", menuName = "AAI Segmentation/Tag Color Table", order = 1)]
public class TagsColorsLookup : ScriptableObject
{
	public List<TagColor> TagColorList;
}