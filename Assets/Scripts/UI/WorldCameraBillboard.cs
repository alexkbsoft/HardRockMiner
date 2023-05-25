using UnityEngine;

namespace Game.GameEngine
{
    [AddComponentMenu("GameEngine/World Camera/World Camera Billboard")]
    [ExecuteAlways]
    public sealed class WorldCameraBillboard : MonoBehaviour
    {
        [SerializeField]
        private bool lookAtStart;
    
        void Update()
        {
            LookAtCamera();        
        }        
        
        public void LookAtCamera()
        {
            var rootPosition = this.transform.position;
            var camera = Camera.main;
            if (camera == null)
            {
                return;
            }

            var cameraRotation = camera.transform.rotation;
            var cameraVector = cameraRotation * Vector3.forward;
            var worldUp = cameraRotation * Vector3.up;
            this.transform.LookAt(rootPosition + cameraVector, worldUp);
        }
    }
}