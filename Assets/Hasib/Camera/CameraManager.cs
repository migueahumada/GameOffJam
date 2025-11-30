using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineCamera _cinemachineCamera;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cinemachineCamera.Priority = 10;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cinemachineCamera.Priority = 0;
        }
    }
}
