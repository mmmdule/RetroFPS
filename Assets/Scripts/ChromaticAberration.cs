
using UnityEngine;

[ExecuteInEditMode]
public class ChromaticAberration : MonoBehaviour
{
    public Vector2 redOffset;
    public Vector2 greenOffset;
    public Vector2 blueOffset;

    private Material material;

	// Use this for initialization
	void Start ()
    {
        material = new Material(Shader.Find("Hidden/ChromaticAberration"));
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        material.SetVector("u_redOffset", redOffset);
        material.SetVector("u_greenOffset", greenOffset);
        material.SetVector("u_blueOffset", blueOffset);
        Graphics.Blit(source, destination, material);
    }
}
