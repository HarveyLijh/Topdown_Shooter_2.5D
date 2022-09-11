using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class adjustSpriteSize : MonoBehaviour
{
    [SerializeField] float scale = 8f;
    SpriteRenderer sp;

    Vector2 origialSize;
    // Start is called before the first frame update
    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.drawMode = SpriteDrawMode.Sliced;
        origialSize = sp.size;
    }

    // Update is called once per frame
    void Update()
    {
        sp.size = new Vector2(origialSize.x * scale, origialSize.y * scale);

    }
}
