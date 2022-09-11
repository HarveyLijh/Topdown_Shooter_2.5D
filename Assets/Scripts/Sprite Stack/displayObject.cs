using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class displayObject : MonoBehaviour
{
    [SerializeField] stackobject stackObject;
    [SerializeField] Vector3 rotation;
    [SerializeField] Quaternion stackRotation;
    [SerializeField] Vector3 stack_scale = new Vector3(1,1,1);
    [SerializeField] Vector3 stack_position = new Vector3(0, 0, 0);
    [SerializeField] float x_scale = 1f;
    [SerializeField] float y_scale = 1f;
    [SerializeField] Vector3 offset = new Vector3(0, 0.05f, 0);
    [SerializeField] int orderInLayer = 0;
    [SerializeField] Material spriteShadowMat;
    [SerializeField] string layerName;
    private GameObject parts;
    Vector2 origialSize;


    public List<GameObject> partList;
    void GenerateStack()
    {

        parts.transform.parent = transform;
        parts.transform.localPosition = Vector3.zero;
        for (int i = stackObject.stack.Count - 1; i >= 0; i--)
        {
            GameObject stackPart = new GameObject("part" + i);
            SpriteRenderer sp = stackPart.AddComponent<SpriteRenderer>();
            sp.drawMode = SpriteDrawMode.Sliced;
            //Debug.Log("Sprite size: " + sp.size.ToString("F2"));
            origialSize = sp.size;
            sp.sortingLayerName = layerName;

            sp.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            sp.receiveShadows = true;
            sp.material = spriteShadowMat;
            sp.sprite = stackObject.stack[i];
            stackPart.transform.parent = parts.transform;
            stackPart.transform.position = Vector3.zero;


            partList.Add(stackPart);
        }
    }

    void Start()
    {
        parts = new GameObject("Parts");
        GenerateStack();
    }
    void draw_stack()
    {
        int s = orderInLayer;
        Vector3 v = Vector3.zero;
        foreach (GameObject part in partList)
        {
            part.transform.localPosition = v;
            SpriteRenderer sp = part.GetComponent<SpriteRenderer>();
            v += offset;
            sp.size = new Vector2(origialSize.x * x_scale, origialSize.y * y_scale);
            part.transform.localRotation = Quaternion.Euler(rotation);

            sp.sortingOrder = s;
            //s += 1;
        }
        parts.transform.localRotation = stackRotation;
        parts.transform.localScale = stack_scale;
        parts.transform.localPosition = stack_position;
    }
    void Update()
    {
        draw_stack();
    }
}
