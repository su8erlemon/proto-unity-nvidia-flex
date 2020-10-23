using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

public class PositionReset : MonoBehaviour
{
    private FlexSoftActor flexSoftActor;
    // Start is called before the first frame update
    void Start()
    {
        flexSoftActor = GetComponent<FlexSoftActor>();
    }

    // Update is called once per frame
    void Update()
    {

        if (flexSoftActor.enabled == false)
        {
            flexSoftActor.enabled = true;
        }

        if (transform.position.y > 4)
        {

            flexSoftActor.enabled = false;
            Vector3 pos = transform.position;
            pos.x = Random.Range(-2.0f, 2.0f);
            pos.y = Random.Range(-3.0f, -8.0f);//-4;
            pos.z = Random.Range(-.3f, .3f);
            transform.position = pos;
        }

    }
}
