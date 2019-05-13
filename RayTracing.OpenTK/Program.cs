using System;
namespace RayTracing.OpenTK
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new Window();
            window.MouseMove += (sender, eventArgs) => window.Title = window.GetPixelColor(eventArgs.Position.X, eventArgs.Position.Y) ?? "RayTracing";
            window.Closing += (sender, e) => Environment.Exit(0);
            
            window.Start();
        }
    }
}
