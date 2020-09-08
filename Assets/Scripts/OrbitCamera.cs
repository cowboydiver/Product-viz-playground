using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    public Transform focus;
    Vector3 focusPoint;

    [SerializeField, Min(0f)]
    public float focusRadius = 1f;

    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.75f;

    [SerializeField, Range(1f, 360f)]
    float rotationSpeed = 90f;

    [SerializeField, Range(1f, 20f)]
    float easingSpeed = 10f;

    [SerializeField, Range(5f, 20f)]
    public float zoomDistance = 5f;

    float targetZoomDistance;

    [SerializeField, Range(0f, 20f)]
    float zoomSpeed = 10f;

    Vector2 orbitAngles = new Vector2(15f, 45f);
    Vector2 targetOrbitAngles = new Vector2();

    public float xMinLimit = -10f;
    public float xMaxLimit = 45f;

    public float zoomMin = 1f;
    public float zoomMax = 20f;


    private void Awake()
    {
        focusPoint = focus.position;
        targetZoomDistance = zoomDistance;
        targetOrbitAngles = orbitAngles;
    }
    
    void LateUpdate()
    {
        UpdateFocusPoint();
        UpdateZoom();
        UpdateRotation();
        ManualRotation();
        ManualZoom();

        Quaternion lookRotation = Quaternion.Euler(orbitAngles);
        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * zoomDistance;

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    private void UpdateRotation()
    {
        const float e = 0.001f;
        Vector2 anglesDifference = targetOrbitAngles - orbitAngles;
        if (anglesDifference.x < -e || anglesDifference.x > e || anglesDifference.y < -e || anglesDifference.y > e)
        {
            orbitAngles = Vector2.Lerp(
                targetOrbitAngles, orbitAngles,
                Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime * easingSpeed)
                );
        }
    }

    private void UpdateZoom()
    {
        if(Mathf.Abs(targetZoomDistance - zoomDistance) > 0.001f)
        {
            zoomDistance = Mathf.Lerp(
                targetZoomDistance, zoomDistance,
                Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime * easingSpeed)
                );
        }
    }

    private void ManualZoom()
    {
        float currentTargetZoom = targetZoomDistance;
        float deltaDistance = Input.GetAxis("Mouse ScrollWheel");
        const float e = 0.001f;
        if(deltaDistance < -e || deltaDistance > e)
        {
            targetZoomDistance += deltaDistance * zoomSpeed;
            targetZoomDistance = Mathf.Clamp(targetZoomDistance, zoomMin, zoomMax);
        }
    }

    private void ManualRotation()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 input = new Vector2(
                        Input.GetAxis("Mouse Y"),
                        Input.GetAxis("Mouse X")
                        );
            const float e = 0.001f;
            if (input.x < -e || input.x > e || input.y < -e || input.y > e)
            {
                targetOrbitAngles +=  rotationSpeed * input * new Vector3(-1f, 1f, 1f) * Time.unscaledDeltaTime;
                targetOrbitAngles.x = Mathf.Clamp(targetOrbitAngles.x, xMinLimit, xMaxLimit);
            }
        }
    }

    void UpdateFocusPoint()
    {
        Vector3 targetPoint = focus.position;
        if(focusRadius > 0f)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            if(distance > focusRadius)
            {
                focusPoint = Vector3.Lerp(targetPoint, focusPoint, focusRadius / distance);
            }
            if(distance > 0.01f && focusCentering > 0f)
            {
                focusPoint = Vector3.Lerp(
                    targetPoint, focusPoint,
                    Mathf.Pow(1f - focusCentering, Time.unscaledDeltaTime * easingSpeed)
                    );
            }
        }
        else
        {
            focusPoint = targetPoint;
        }
    }
}
