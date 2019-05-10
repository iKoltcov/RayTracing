using System.Numerics;
using RayTracing.Entities.Interfaces;
namespace RayTracing.Results
{
    public class SceneIntersectResult
    {
        public IEssence EssenceIntersect { get; set; }

        public Vector3 Point { get; set; }
    }
}
