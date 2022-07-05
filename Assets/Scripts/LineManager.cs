using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.AR;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class LineManager : ARBaseGestureInteractable
{
    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private Pose pose;
    private LineRenderer line;
    private int pointCount = 0;
    public bool continuous;

    [SerializeField] private Image scanGIF;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private GameObject point;
    [SerializeField] private Camera aRCamera;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private TextMeshPro text;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private TextMeshProUGUI helpText;

    private void FixedUpdate()
    {
        if (crosshair.activeSelf)
            CrosshairCalculation();
    }

    public void ModeToggle()
    {
        continuous = !continuous;

        if (continuous)
            modeText.text = "Discrete";
        else
            modeText.text = "Continuous";
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnFinishPlacement()
    {
        crosshair.SetActive(false);
    }

    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.targetObject == null || gesture.targetObject.layer == 9)
            return true;
        return false;
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        if (gesture.isCanceled || !crosshair.activeSelf)
            return;

        if (gesture.targetObject != null && gesture.targetObject.layer != 9)
            return;

        if (IsPointerOverUI(gesture))
            return;

        if (GestureTransformationUtility.Raycast(gesture.startPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            var hit = hits[0];

            if (Vector3.Dot(Camera.main.transform.position - hit.pose.position, hit.pose.rotation * Vector3.up) < 0)
                return;

            GameObject placedObj = Instantiate(point, pose.position, pose.rotation);
            var anchorObj = new GameObject("PlacementAnchor");
            anchorObj.transform.SetPositionAndRotation(pose.position, pose.rotation);
            placedObj.transform.parent = anchorObj.transform;

            Destroy(helpText);
            Destroy(scanGIF);
            DrawLine(placedObj);
        }
    }

    bool IsPointerOverUI(TapGesture touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void CrosshairCalculation()
    {
        Vector3 origin = aRCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

        if (GestureTransformationUtility.Raycast(origin, hits, TrackableType.PlaneWithinPolygon))
        {
            pose = hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.eulerAngles = new Vector3(90, 0, 0);
        }
    }

    private void DrawLine(GameObject gameObject)
    {
        pointCount++;

        if (pointCount < 2)
        {
            line = Instantiate(lineRenderer);
            line.positionCount = 1;
        }

        else
        {
            line.positionCount = pointCount;

            if (!continuous)
                pointCount = 0;
        }

        line.SetPosition(line.positionCount - 1, pose.position);

        if (line.positionCount > 1)
        {
            Vector3 pointA = line.GetPosition(line.positionCount - 1);
            Vector3 pointB = line.GetPosition(line.positionCount - 2);

            float distance;

            string measurement = PlayerPrefs.GetString("Measurement");

            if (string.IsNullOrEmpty(measurement) == true)
            {
                measurement = "m";
                distance = GetDistance(pointA, pointB, measurement);
            }
            else
                distance = GetDistance(pointA, pointB, measurement);

            TextMeshPro distanceText = Instantiate(text);
            distanceText.text = "± " + distance.ToString("F2") + " " + measurement;

            Vector3 direction = (pointB - pointA);
            Vector3 normal = line.transform.up;
            Vector3 update = Vector3.Cross(direction, normal).normalized;
            Quaternion rotation = Quaternion.LookRotation(-normal, update).normalized;

            distanceText.transform.rotation = rotation;
            distanceText.transform.position = (pointA + direction * 0.5f) + update * 0.05f;
        }
    }

    float GetDistance(Vector3 pointA, Vector3 pointB, string measurement)
    {
        float m = Vector3.Distance(pointA, pointB);

        switch (measurement)
        {
            case "mm":
                return m * 1000.00f;
            case "cm":
                return m * 100.00f;
            case "m":
                return m / 1.0f;
            case "km":
                return m / 1000.0f;
            case "in":
                return m * 39.37f;
            case "ft":
                return m * 3.681f;
            default:
                break;
        }

        return 0;
    }
}
