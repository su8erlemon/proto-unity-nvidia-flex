using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*

    to convert Geometory verices to Texture

    1024x1024 Texture which contains normalized geometory vertices based on baseMesh

    _geometoryTexture = new S8GeometryTexture(baseMesh);

    // get number of the vertices
    _geometoryTexture.getCount(); 

    // get Texture
    _geometoryTexture.getTexture(); 


    // in shader
    // get vertex xyz position 
    float3 geometory(float index){
        float dd = (1.0/1024.0) * 0.5;
        float y = (1.0/1024.0) * index;
        return (tex2Dlod (_GeometoryTexture, float4(y+dd,dd,0.0,0.0)).xyz-0.5)*2.0;
    }

    float3 xyz = geometory(index);

*/

public class S8GeometryTexture{
    
    static float MAX_COUNT = 10000;
    private Texture2D _geometoryTexture; 
    private int _num;
    
    public S8GeometryTexture(Mesh baseMesh){
        int index = 0;

        Vector3[] v3 = baseMesh.vertices;
        // Debug.Log(baseMesh.bounds.size.x);

        int increment = 1;
        if( baseMesh.vertexCount > MAX_COUNT ){
            increment = (int)Mathf.Floor(baseMesh.vertexCount/MAX_COUNT);
        }

        List<Vector3> v3s = new List<Vector3>();
        for( int i = 0; i < baseMesh.vertexCount; i += increment ){
            bool isExist = false;
            v3s.ForEach(v=>{
               if( v == v3[i] )isExist = true; 
            });
            if( isExist == false ){
                // Debug.Log(v3[i]);
                v3s.Add(v3[i]);
            }
        }
        
        
        _num = v3s.Count;
        if( _num >= 10000 )_num = 10000;

        float maxScale = Mathf.Max(Mathf.Max(baseMesh.bounds.size.x,baseMesh.bounds.size.y),baseMesh.bounds.size.z); 

        _geometoryTexture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false, true);
        var geometoryTextureData = _geometoryTexture.GetRawTextureData<Color32>();
        index = 0;
        for (float y = 0; y < _geometoryTexture.height; y++)
        {
            for (float x = 0; x < _geometoryTexture.width; x++)
            {
                // if( index >= v3s.Count )continue;
                geometoryTextureData[index] = new Color32(
                    (byte)((0.5f+v3s[index%_num].x*0.5f)*255.0f),
                    (byte)((0.5f+v3s[index%_num].y*0.5f)*255.0f),
                    (byte)((0.5f+v3s[index%_num].z*0.5f)*255.0f),
                    255
                );
                index += 1;
            }
        }
        _geometoryTexture.wrapMode = TextureWrapMode.Clamp;
        _geometoryTexture.Apply();

    }

    public int getCount(){
        return _num;
    }

    public Texture2D getTexture(){
        return _geometoryTexture;
    }

}
