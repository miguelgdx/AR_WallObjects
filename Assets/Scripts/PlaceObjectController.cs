using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceObjectController : MonoBehaviour
{
    [SerializeField]
    GameObject _picturePrefab;
    [SerializeField]
    GameObject _tvPrefab;
    [SerializeField]
    Text _canvasText;
    [SerializeField]
    Text _instText;
    [SerializeField]
    Camera ARCamera;
    [SerializeField]
    GameObject ToggleButton;
    bool toggledPrefabType = false;
    ARRaycastManager m_RaycastManager;
    ARPlaneManager planeManager;
    LayerMask objMask;
    float _touchStartTime = 0f;
    const float LONGPRESSTIME = 1.5f;
    GameObject _currentSelectedObj;
    GameObject _objToSpawn;

    // Array with all hits
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Awake()
    {
        m_RaycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        _objToSpawn = _picturePrefab;
        objMask = LayerMask.GetMask("WallObjects");
        _instText.text = "Current prefab: " + _objToSpawn.name;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Prevent raycasting objects behind the prefab switching button.
            if (RectTransformUtility.RectangleContainsScreenPoint(ToggleButton.GetComponent<RectTransform>(), touch.position))
            {
                return;
            }

            // Get time as we start touch
            if (touch.phase == TouchPhase.Began)
            {
                _touchStartTime = Time.time;
            }

            // Remove any current selection if touch ended.
            if (touch.phase == TouchPhase.Ended)
            {
                _currentSelectedObj = null;
            }

            // Delete object if short tap.
            if (touch.phase == TouchPhase.Ended && isShortTap())
            {
                RaycastHit hit;
                Ray ray = ARCamera.ScreenPointToRay(touch.position);
                // Check if we hit an existing object first within the wall mask.
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, objMask))
                {
                    Transform objectHit = hit.transform;
                    textOutput("Touched: " + objectHit.gameObject.ToString());
                    // If we hit a WallObject, let it be destroyed.
                    Destroy(objectHit.gameObject);
                    return;
                }
                else { 
                    // If we only hit a plane with short touch instantiate the prefab
                    if (m_RaycastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                    {
                        Pose hitPose = s_Hits[0].pose;
                        GameObject spawnObj;
                        spawnObj = Instantiate(_objToSpawn, hitPose.position, hitPose.rotation);

                        // Retrieve the plane that was hit.
                        ARPlane planeHit = planeManager.GetPlane(s_Hits[0].trackableId);
                        // Get the plane normal so we know where it's facing at.
                        Vector3 planeNormal = planeHit.normal;
                        // Make the instanced object rotate into the same direction as the plane
                        Quaternion destRotation = Quaternion.LookRotation(-planeNormal);
                        spawnObj.transform.rotation = destRotation;
                    }
                }
            }
            else
            {
                // If we are long touching an object, rotate it.
                if (touch.phase != TouchPhase.Ended && !isShortTap())
                {
                    RaycastHit hit;
                    Ray ray = ARCamera.ScreenPointToRay(touch.position);

                    if (_currentSelectedObj == null && Physics.Raycast(ray, out hit, Mathf.Infinity, objMask))
                    {
                        Transform objectHit = hit.transform;
                        _currentSelectedObj = objectHit.gameObject;
                        textOutput("Long tapped: " + _currentSelectedObj.name);
                    }

                    // Make the current selected object rotate it on the Z axis clockwise.
                    _currentSelectedObj.transform.RotateAround(_currentSelectedObj.transform.position, -_currentSelectedObj.transform.forward, Time.deltaTime * 90f);
                }

            }
        }
    }

    // Checks if the user performed a short tap.
    public bool isShortTap()
    {
        return ((Time.time - _touchStartTime)  < LONGPRESSTIME);
    }

    // Switches between picture frame or TV
    public void togglePrefabType()
    {
        toggledPrefabType = !toggledPrefabType;
        if (toggledPrefabType)
            _objToSpawn = _tvPrefab;
        else
            _objToSpawn = _picturePrefab;

        _instText.text = "Current prefab: " + _objToSpawn.name;
    }

    // Text output to screen (if we can't have console).
    public void textOutput(String text)
    {
        _canvasText.text = text;
    }
}
