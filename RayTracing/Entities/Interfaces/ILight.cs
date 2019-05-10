namespace RayTracing.Entities.Interfaces
{
    public interface ILight : ICoordinatable
    {
        float Intensity { get; set; }
    }
}
