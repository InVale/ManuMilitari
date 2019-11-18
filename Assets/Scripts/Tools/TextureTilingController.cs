using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Renderer))]
public class TextureTilingController : MonoBehaviour
{
    public float textureToMeshZ = 1f; // Use this to constrain texture to a certain size

    // Use this for initialization
    void Start()
    {
        UpdateTiling();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.hasChanged)
            UpdateTiling();
    }

    void OnValidate ()
    {
        UpdateTiling();
    }

    [ContextMenu("UpdateTiling")]
    void UpdateTiling()
    {
        Renderer _renderer = GetComponent<Renderer>();
        Texture _texture = _renderer.sharedMaterial.mainTexture;

        // A Unity plane is 10 units x 10 units
        float planeSizeX = 10f;
        float planeSizeZ = 10f;

        // Figure out texture-to-mesh width based on user set texture-to-mesh height
        float textureToMeshX = ((float)_texture.width / _texture.height) * textureToMeshZ;

        _renderer.sharedMaterial.mainTextureScale = new Vector2(planeSizeX * gameObject.transform.lossyScale.x / textureToMeshX, planeSizeZ * gameObject.transform.lossyScale.z / textureToMeshZ);
    }
}