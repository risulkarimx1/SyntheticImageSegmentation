using UnityEngine;
using UnityEngine.Serialization;

public class RenderStatus : MonoBehaviour
{
	private ObjectListMaker _visibleObjectListMaker;
	private MeshFilter _meshFilter;

	[SerializeField] private bool _isVisible;

	public void SetVisibleRendererList(ObjectListMaker visibleObjectListMaker)
	{
		_visibleObjectListMaker = visibleObjectListMaker;
		_meshFilter = GetComponent<MeshFilter>();
	}
	private void OnBecameVisible()
	{
		_isVisible = true;
		_visibleObjectListMaker.AddRenderer(_meshFilter);
	}
	void OnBecameInvisible()
	{
		_isVisible = false;
		_visibleObjectListMaker.RemoveRenderer(_meshFilter);
	}
}