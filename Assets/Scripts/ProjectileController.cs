using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

    private Color color = new Color(255, 0, 0);
    private float lifetime = 10;
    private float age = 0;
    private Rigidbody2D rb2d;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        age += Time.deltaTime;
        if (age > lifetime)
            Destroy(this.gameObject);
	}

    // When added to an object, draws colored rays from the
    // transform position.
    private float lineSegmentLength = 0.01f;
    private float lineFrequency = 333f;

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transformaddw
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINE_STRIP);
        GL.Color(new Color(color.r, color.g, color.b, 0.8F));

        Vector2 trailPos = new Vector2();
        Vector2 lineVector = -rb2d.velocity.normalized * lineSegmentLength;
        Vector2 orthoVector = new Vector2(lineVector.y, -lineVector.x);

        int lineCount = (int) (age * lineFrequency * Mathf.PI * 2);
        for (int i = 0; i < lineCount; ++i)
        {
            GL.Vertex3(trailPos.x, trailPos.y, 0);
            trailPos += (
                (lineVector) +
                (orthoVector * Mathf.Sin((i / lineFrequency) * 2 * Mathf.PI + ((Time.time - (int) Time.time) * 2 * Mathf.PI)))
            );
        }
        GL.End();
        GL.PopMatrix();
    }

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

}