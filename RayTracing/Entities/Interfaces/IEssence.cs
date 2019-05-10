using System.Numerics;
namespace RayTracing.Entities.Interfaces
{
    public interface IEssence : ICoordinatable
    {
        Vector3? CheckCollision(RayEntity ray);

        MaterialEntity Material { get; set; }
    }
}
