using UnityEngine;

public class ObjFade : MonoBehaviour {
    public Material material;
    public float distance;
    private Transform camera;
    public float dist;
    private void Start()
    {
        camera = Camera.main.transform;
        transform.GetComponent<MeshRenderer>().material = transform.parent.GetComponent<MeshRenderer>().material; ;
        material = transform.GetComponent<MeshRenderer>().material;

        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.SetFloat("_Mode", 2);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
        
        transform.GetComponent<MeshRenderer>().material = material;
    }

    private void Update()
    {
        if (material == null) return;

        Color c = material.color;
        float d = Vector3.Distance(this.transform.position, camera.transform.position);
        d /= distance;
        c.a = Mathf.Min(d, 1);
        dist = d;
        material.color = c;
    }
}
