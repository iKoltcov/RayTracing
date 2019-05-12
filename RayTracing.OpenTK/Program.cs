using System;
namespace RayTracing.OpenTK
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new Window();
            window.Closing += (sender, e) => Environment.Exit(0);
        }
    }
}
