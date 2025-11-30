using System;
using System.Collections;
using UnityEngine;

namespace CoffeeMachineArm
{
    public class MeshBlender : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshAFilter;
        [SerializeField] private MeshFilter _meshBFilter;

        private Mesh _mesh;            
        private Vector3[] _aVerts;
        private Vector3[] _bVerts;
        private Vector3[] _blendedVerts;

        private void Start()
        {
            _mesh = Instantiate(_meshAFilter.sharedMesh);
            _mesh.name = "BlendMesh";

            GetComponent<MeshFilter>().sharedMesh = _mesh;

            _aVerts = _meshAFilter.sharedMesh.vertices;
            _bVerts = _meshBFilter.sharedMesh.vertices;

            if (_aVerts.Length != _bVerts.Length)
            {
                Debug.LogError("Meshes do NOT have the same vertex count!");
                return;
            }

            _blendedVerts = new Vector3[_aVerts.Length];
            
            StartCoroutine(BlendRoutine());
        }

        private void Blend(Single t)
        {
            for (Int32 i = 0; i < _blendedVerts.Length; i++)
                _blendedVerts[i] = Vector3.Lerp(_aVerts[i], _bVerts[i], t);

            _mesh.vertices = _blendedVerts;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }

        private IEnumerator BlendRoutine()
        {
            Single t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime;
                Blend(t);
                yield return null;
            }
        }
    }
}