using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class S8CinemaCam : MonoBehaviour
{
    public enum CamType // your custom enumeration
    {
        Free, 
        LookAt, 
        Follow,
        CameraTargets
    };
    
    public CamType camType = CamType.Free;  // this public var should appear as a drop down

    [SerializeField] 
    public bool jitterMove = true;

    public Transform target; 
    private GameObject dummyTarget;

    public float duration = 1.0f;
    private float _time = 0.0f;
    private float _camPer = 0.0f;

    private float _angleAttenRate = 2.0f; 
    private float _jitterFactor = 0.0f;
    private  float _jitterSpeed = 0.5f;
    private  float _jitterAmp = 2.0f;

    public float _followDistance = 7.0f; 
    public float _followHeight = 0.0f;
    
    public float targetTrack = 0.0f;
    public float targetDolly = 0.0f;
    public float targetUpdown = 0.0f;
    public float targetPan = 0.0f;
    public float targetTilt = 0.0f;
    public float targetRoll = 0.0f;

    public float currentTrack = 0.0f;
    public float currentDolly = 0.0f;
    public float currentUpdown = 0.0f;
    public float currentPan = 0.0f;
    public float currentTilt = 0.0f;
    public float currentRoll = 0.0f;

    public float track = 0.0f;
    public float dolly = 0.0f;
    public float updown = 0.0f;
    public float pan = 0.0f;
    public float tilt = 0.0f;
    public float roll = 0.0f;

    public List<Camera> cameraPosList = new List<Camera>();
    private Vector3 _targetCameraPos;
    private Quaternion _targetCameraRot;

    private Vector3 _currentPos;
    private Quaternion _currentRot;

    [SerializeField]
    public AnimationCurveSpeed curve;

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget  = new GameObject();
        if( cameraPosList.Count > 0 ){
            _targetCameraPos = cameraPosList[0].transform.position;
            _targetCameraRot = cameraPosList[0].transform.rotation;
        }        
    }

    // Update is called once per frame
    void Update()
    {


        _time += Time.deltaTime;
        if( curve ){
            _camPer += curve.Evaluate(_time,duration);
        }
        
        // Debug.Log(_time +"/"+_time2);
        if( camType.Equals(CamType.Follow ) ){
            var pos = target.position + new Vector3(0.0f, _followHeight, -_followDistance); // 本来到達しているべきカメラ位置
            pos += transform.right * track;
            pos += transform.forward * dolly;
            pos += transform.up * updown;
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * _angleAttenRate); // Lerp減衰
            dummyTarget.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }else if( camType.Equals(CamType.LookAt )){
            dummyTarget.transform.position = transform.position; 
            dummyTarget.transform.LookAt(target);
        }else if( camType.Equals(CamType.CameraTargets)){
            var pos = _targetCameraPos;

            track  = Mathf.Lerp(currentTrack,targetTrack,_camPer);
            dolly  = Mathf.Lerp(currentDolly,targetDolly,_camPer);
            updown = Mathf.Lerp(currentUpdown,targetUpdown,_camPer);
            pan    = Mathf.Lerp(currentPan,targetPan,_camPer);
            tilt   = Mathf.Lerp(currentTilt,targetTilt,_camPer);
            roll   = Mathf.Lerp(currentRoll,targetRoll,_camPer);

            pos += transform.right * track;
            pos += transform.forward * dolly;
            pos += transform.up * updown;

            transform.position = Vector3.Slerp(_currentPos, pos,  _camPer ); 

            var rot = _targetCameraRot;
            rot *= Quaternion.Euler(tilt,pan,roll);
            dummyTarget.transform.rotation = rot;
        }

      
        if( jitterMove ){
            _jitterFactor += ( 1.0f - _jitterFactor ) / 5.0f;
        }else{
            _jitterFactor += ( 0.0f - _jitterFactor ) / 5.0f;
        }

        var t = Time.time * _jitterSpeed;
        var nx = (Mathf.PerlinNoise(t,         t + 5.0f ) -0.5f) * _jitterAmp * _jitterFactor;
        var ny = (Mathf.PerlinNoise(t + 10.0f, t + 15.0f) -0.5f) * _jitterAmp * _jitterFactor;
        var nz = (Mathf.PerlinNoise(t + 25.0f, t + 20.0f) -0.5f) * _jitterAmp * _jitterFactor * 0.5f;
        var noise = new Vector3(nx, ny, nz);
        var noiseRot = Quaternion.Euler(noise.x, noise.y, noise.z);

        if( camType.Equals(CamType.CameraTargets) ){
            // transform.rotation = Quaternion.Slerp(transform.rotation, noiseRot * dummyTarget.transform.rotation,  Time.deltaTime * per );
            //transform.rotation = Quaternion.Slerp(_currentRot, noiseRot * dummyTarget.transform.rotation,  per2 );
            transform.rotation = Quaternion.Slerp(_currentRot, noiseRot * dummyTarget.transform.rotation,  _camPer );  
        }else{
            transform.rotation = Quaternion.Slerp(transform.rotation, noiseRot * dummyTarget.transform.rotation, Time.deltaTime * _angleAttenRate );
        }
        
        // if (Input.GetKeyDown("1")) {
        //     targetTo(0);
        // } 

        // if (Input.GetKeyDown("2")) {
        //     targetTo(1);
        // } 

        // if (Input.GetKeyDown("3")) {
        //     targetTo(2);
        // } 

        // if (Input.GetKeyDown("4")) {
        //     targetTo(3);
        // } 

        // if (Input.GetKeyDown("t")) {
        //     Debug.Log("track");
        // } 
    }

    public void targetTo(int index){
        if( index >= cameraPosList.Count ) return; 
        _currentPos = transform.position;
        _currentRot = transform.rotation;
        _time = 0.0f;
        _camPer = 0.0f;
        _targetCameraPos = cameraPosList[index].transform.position;
        _targetCameraRot = cameraPosList[index].transform.rotation;

        currentTrack = track;
        currentDolly = dolly;
        currentUpdown = updown;
        currentPan = pan;
        currentTilt = tilt;
        currentRoll = roll;
    }

    public void shake(){

    }
}
