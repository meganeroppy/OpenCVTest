using UnityEngine;
using System.Collections;

public class SetMeshRendererSortingLayer : MonoBehaviour
{
	[SerializeField]
	private Renderer myRenderer;

	[SerializeField]
	private string sortingLayerName;

	[SerializeField]
	private int sortingOrder;

	public void Start()
	{
		myRenderer.sortingLayerName = sortingLayerName;
		myRenderer.sortingOrder = sortingOrder;
	}
}