using UnityEngine;
using System.Collections;

public class TextureAnimator : MonoBehaviour
{
	public Material material;
	public Texture[] textures;
	public float delay = 0.3f;
	
	private int index;

	void Start()
	{
		if (!material && GetComponent<Renderer>())
			material = GetComponent<Renderer>().material;
		
		if (material && textures.Length > 0)
			StartCoroutine(Animate());
	}
	
	IEnumerator Animate()
	{
		while (true)
		{
			material.mainTexture = textures[index];
			index = (index + 1) % textures.Length;
			yield return new WaitForSeconds(delay);
		}
	}
}
