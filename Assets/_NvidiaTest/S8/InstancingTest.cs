using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancingTest : MonoBehaviour
{
    public AnimationCurveSpeed scaleCurve; 
    public GameObject instancingPrefab;
    public int num = 10;
    public Mesh baseMesh;
    public S8EasingTexture s8EasingTexture;

    private GameObject _instancingObject;
    private Material _instancingMat;
    private S8GeometryTexture _geometoryTexture;
    
    private float _time = 0.0f;

    void Start()
    {
        _geometoryTexture = new S8GeometryTexture(baseMesh);

        // get Texture
        num = _geometoryTexture.getCount();
        
        for (var i = 0; i < num; i++){
            _instancingObject = Instantiate(instancingPrefab, transform.position, transform.rotation, transform);         
        }
    
        _instancingMat = _instancingObject.GetComponent<Renderer>().sharedMaterial;
        
        _instancingMat.SetTexture("_GeometoryTexture", _geometoryTexture.getTexture());
        _instancingMat.SetTexture("_EasingTexture", s8EasingTexture.getTexture());   
    }

    // Update is called once per frame
    void Update()
    {   
        _instancingMat.SetTexture("_EasingTexture", s8EasingTexture.getTexture());
        
        _time += Time.deltaTime;
        if( _time > 1.0f ){
            _time = 0.0f;
            _instancingMat.SetInt("_Type", (int)Random.Range(0,5));
        }
        _instancingMat.SetFloat("_ParticleTime", _time);
        _instancingMat.SetFloat("_ScaleCurve", scaleCurve.curve.Evaluate(_time%1.0f));
    }

}
