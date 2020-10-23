using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCurveSpeed : MonoBehaviour
{
    public AnimationCurve curve;
    public bool updateEveryFrame = true;
    private float _ratio = 1.0f;
    private float _areaSize = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        calcAreaSizeRatio();
    }

    void Update(){
        if( updateEveryFrame ) calcAreaSizeRatio();
    }

    // its gonna be 1 adding return value while duration time
    public float Evaluate(float time,float duration){
        return curve.Evaluate(time/duration) / duration * (Time.deltaTime / _areaSize);
    }

    private void calcAreaSizeRatio(){
        _areaSize = 0.0f;
        float prevX = 0.0f;;
        for( int i = 0 ; i <= 1000; i++ ){
            // Vector2 xy = curve.Evaluate(i/1000.0f);
            // _areaSize += xy.y * (xy.x-prevX);
            float aa = curve.Evaluate(i/1000.0f);
            _areaSize += aa * 1/1000.0f;
            // prevX = xy.x;
        }
    }

    private Vector2 getValueFromAnimationCurve(float time){

        int firstIndex = 0;
        int secondIndex = 1;

        for( int i = 0; i < curve.keys.Length; i++ ){
            if( time > curve.keys[i].time ){
                firstIndex = i;
                secondIndex = i+1;
            }
        }

		var duration = curve.keys[secondIndex].time - curve.keys[firstIndex].time;
		var t = (time - curve.keys[firstIndex].time) / duration;

        var p0 = new Vector2(curve.keys[firstIndex].time                                             , curve.keys[firstIndex].value                                                                                    );
		var p1 = new Vector2(curve.keys[firstIndex].time + curve.keys[firstIndex].outWeight*duration , curve.keys[firstIndex].value + curve.keys[firstIndex].outWeight * duration * curve.keys[firstIndex].outTangent );
		var p2 = new Vector2(curve.keys[secondIndex].time - curve.keys[secondIndex].inWeight*duration , curve.keys[secondIndex].value - curve.keys[secondIndex].inWeight * duration * curve.keys[secondIndex].inTangent );
		var p3 = new Vector2(curve.keys[secondIndex].time                                             , curve.keys[secondIndex].value                                                                                   );

        return new Vector2(
               p0.x * (1-t) * (1-t) * (1-t) 
             + p1.x * 3f * t * (1-t) * (1-t)
             + p2.x * 3f * t * t * (1-t)
             + p3.x * t * t * t,
               
               p0.y * (1-t) * (1-t) * (1-t) 
             + p1.y * 3f * t * (1-t) * (1-t)
             + p2.y * 3f * t * t * (1-t)
             + p3.y * t * t * t
        );
             
    }


}
