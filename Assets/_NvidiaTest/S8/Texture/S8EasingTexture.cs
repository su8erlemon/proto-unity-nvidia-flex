using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*

    to convert AnimationCurve to Texture

    // get Texture
    s8EasingTexture.getCount(); 

    // in shader 
    // id: animationCurves index
    // t: 0~1
    float easing(float id, float t){
        float dd = (1.0/1024.0) * 0.5;
        return tex2Dlod (_EasingTexture, float4(t+dd,dd,0.0,0.0)).r;
    }
    float value = easing(0, 0.5);

*/

public class S8EasingTexture : MonoBehaviour
{
    
    public AnimationCurve[] animationCurves;

    private Texture2D _texture; 

    // Start is called before the first frame update
    void Awake()
    {
        _texture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);
        var data = _texture.GetRawTextureData<Color32>();
        int index = 0;
        for (int i = 0; i < animationCurves.Length; i++)
        {
            for (float x = 0; x < _texture.width; x++)
            {
                data[index++] = new Color32(
                    (byte)(animationCurves[i].Evaluate(x / _texture.height) * 255),
                    0,
                    0,
                    255
                );
            }
        }
        _texture.wrapMode = TextureWrapMode.Clamp;
        _texture.Apply();

    }

    // Update is called once per frame
    void Update()
    {
        var data = _texture.GetRawTextureData<Color32>();
        int index = 0;
        for (int i = 0; i < animationCurves.Length; i++)
        {
            for (float x = 0; x < _texture.width; x++)
            {
                data[index++] = new Color32(
                    (byte)(animationCurves[i].Evaluate(x / _texture.height) * 255),
                    0,
                    0,
                    255
                );
            }
        }
        _texture.wrapMode = TextureWrapMode.Clamp;
        _texture.Apply();
    }

    public Texture2D getTexture(){
        return _texture;
    }
}
