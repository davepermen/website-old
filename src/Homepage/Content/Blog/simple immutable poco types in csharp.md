# Simple Immutable POCO Types in C#

I wrote some simple POCO that’s immutable, and flexible in modification on the fly by constructing a new one.

C# 6 will make it a bit less verbose (but not that much yet ☹)

# Some usage examples

```csharp
var first = new Camera(
    position: Vector3.Zero,
    target: Vector3.UnitZ,
    up: Vector3.UnitY,
    eyeDistance: 1
);
var modifiedEyeDistance = new Camera(
    previous: first,
    eyeDistance: 2
);
var modifiedPosition = new Camera(
    previous: modifiedEyeDistance,
    position: modifiedEyeDistance.Position + Vector3.UnitY - Vector3.UnitZ
);
```

# Class used in the examples, and it’s implementation

The class has two constructors, one setting all values (and require to set all), and one using a previous class instance.

On the second constructor, all parameters are optional, except for the previous class. This, combined with named parameters, allows to just change the part of the state one wants.

```csharp
namespace Runtime.Views
{
    using System.Numerics;
    public class Camera
    {
        readonly Vector3 position;
        readonly Vector3 target;
        readonly Vector3 up;
        readonly float eyeDistance;
        public Vector3 Position => position;
        public Vector3 Target => target;
        public Vector3 Up => up;
        public float EyeDistance => eyeDistance;
        public Camera(Vector3 position, Vector3 target, Vector3 up,
                                                       float eyeDistance)
        {
            this.position = position;
            this.target = target;
            this.up = up;
            this.eyeDistance = eyeDistance;
        }
        public Camera(Camera previous, Vector3? position = null,
                              Vector3? target = null, Vector3? up = null,
                                               float? eyeDistance = null)
        {
            this.position = position ?? previous.position;
            this.target = target ?? previous.target;
            this.up = up ?? previous.up;
            this.eyeDistance = eyeDistance ?? previous.eyeDistance;
        }
    }
}
```

# Optional different syntax

One could consider, instead of the second constructor, a Modified method (choose what ever name you like) that returns a new object.

For demo purposes, I’ve implemented Modified using the first constructor as an extension method. Usage below is quite nice to read, I think. I’m considering switching to that approach now (while writing the blogpost.. horray)

```csharp
public static class CameraHelper
{
    public static Camera Modified(this Camera previous,
                        Vector3? position = null, Vector3? target = null,
                           Vector3? up = null, float? eyeDistance = null)
    {
        return new Camera(
            position: position ?? previous.Position,
            target: target ?? previous.Target,
            up: up ?? previous.Up,
            eyeDistance: eyeDistance ?? previous.EyeDistance
        );
    }
}
```

With the new 'Modified' extension method, usage is like this

```csharp
var first = new Camera(
    position: Vector3.Zero,
    target: Vector3.UnitZ,
    up: Vector3.UnitY,
    eyeDistance: 1
);
var modifiedEyeDistance = first.Modified(
    eyeDistance: 2
);
var modifiedPosition = modifiedEyeDistance.Modified(
    position: modifiedEyeDistance.Position + Vector3.UnitY - Vector3.UnitZ
);
```