using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [System.Serializable]
    public class CameraPosition
    {
        public bool isRight = true;
    }
    [SerializeField]
    public CameraPosition cameraPosition;

    [System.Serializable]
    public class CameraSettings
    {
        public Vector3 offset_right;
        public Vector3 offset_left;
        public float followSpeed = 10f;
        public float rotationSpeed = 10f;
        public float speedX = 5f;
        public float speedY = 5f;

        public float maxWallDistance = 0.15f;
        public float hideMeshWhenDistance = 0.5f;
        public float maxAngle = 60f;
        public float minAngle = -50f;
    }
    [SerializeField]
    public CameraSettings cameraSettings;

    [System.Serializable]
    public class InputSettings
    {
        public float mouseX;
        public float mouseY;
        public bool switchCamera;
    }
    [SerializeField]
    public InputSettings inputSettings;

    Camera mainCam;
    GameObject pivot;
    public Transform player;
    Vector3 desiredPos;

    float newX, newY;

    public LayerMask wallLayer;

    // Start is called before the first frame update
    void Start()
    {   
        if(!player)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        mainCam = Camera.main;
        pivot = transform.GetChild(0).gameObject;
        cameraPosition.isRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        RotateCamera();
        CheckWalls();
        MeshRenderer();
    }

    private void LateUpdate()
    {
        Follow();
        SetShoulderPos();
    }

    void GetInput()
    {
        inputSettings.mouseX = Input.GetAxis("Mouse X");
        inputSettings.mouseY = Input.GetAxis("Mouse Y");
        inputSettings.switchCamera = Input.GetButtonDown("Shoulder");
    }

    void Follow()
    {   
        desiredPos = player.position;
        
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * cameraSettings.followSpeed);
    }

    void SetShoulderPos()
    {
        if(inputSettings.switchCamera)
        {   
            cameraPosition.isRight = !cameraPosition.isRight;
        }
    }

    void RotateCamera()
    {
        newX += inputSettings.mouseX * cameraSettings.speedX;
        newY += inputSettings.mouseY * cameraSettings.speedY;

        Vector3 eulerAngle = new Vector3();
        eulerAngle.x = - newY;
        eulerAngle.y = newX;

        newX = Mathf.Repeat(newX, 360f);
        newY = Mathf.Clamp(newY, cameraSettings.minAngle, cameraSettings.maxAngle);

        Quaternion rot = Quaternion.Slerp(pivot.transform.localRotation, Quaternion.Euler(eulerAngle), Time.deltaTime * cameraSettings.rotationSpeed);
        pivot.transform.localRotation = rot;
    }

    void CheckWalls()
    {
        RaycastHit hit;
        Transform maincamT = mainCam.transform;
        Vector3 camPos = maincamT.position;
        Vector3 pivotPos = pivot.transform.position;
        Vector3 dir = camPos - pivotPos;
        Vector3 startPos = pivotPos;

        float distance = Mathf.Abs( cameraPosition.isRight ? cameraSettings.offset_right.z : cameraSettings.offset_left.z );

        //Debug.Log("distance :" + distance);

        if(Physics.SphereCast(startPos, cameraSettings.maxWallDistance, dir, out hit, distance, wallLayer))
        {
            MoveCameraUp(hit, pivotPos, dir, maincamT);
        }
        else
        {
            if(cameraPosition.isRight)
            {
                PositionCamera(cameraSettings.offset_right);
            }
            else
            {
                PositionCamera(cameraSettings.offset_left);
            }
        }
    }

    void MoveCameraUp(RaycastHit hit, Vector3 pivotPos, Vector3 dir, Transform camT)
    {
        float hitD = hit.distance;
        Vector3 center = pivotPos + (dir.normalized * hitD);
        camT.position = center;
    }

    void PositionCamera(Vector3 offset)
    {
        mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, offset, Time.deltaTime * cameraSettings.followSpeed);
    }

    void MeshRenderer()
    {
        SkinnedMeshRenderer[] skin = player.GetComponentsInChildren<SkinnedMeshRenderer>();

        Vector3 campos = mainCam.transform.position;
        Vector3 playerPos = player.position;

        float d = Vector3.Distance(campos, (playerPos + player.up));
        Debug.Log((playerPos + player.up));
        Debug.Log(campos);
        Vector3 der = campos - (playerPos + player.up);
        Debug.Log(d + "=" + der);

        int l = skin.Length;

        if(l > 0)
        {
            for(int i = 0; i<l; i++)
            {
                if(d < cameraSettings.hideMeshWhenDistance)
                {
                    skin[i].enabled = false;
                }
                else
                {
                    skin[i].enabled = true;
                }
            }
        }
    }
}
