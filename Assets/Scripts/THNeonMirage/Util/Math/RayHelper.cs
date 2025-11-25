using UnityEngine;

namespace THNeonMirage.Util.Math
{
    public static class RayHelper
    {
        public static bool CheckMouseClickHit(Camera camera, out RaycastHit hit)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var rayHit))
            {
                hit = rayHit;
                return true;
            }
            hit = default;
            return false;
        }
        
    }
}