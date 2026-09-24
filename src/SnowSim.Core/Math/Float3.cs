using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SnowSim.Core.Maths
{
    // Minimal blittable vector types for Core formulas. Deliberately not
    // System.Numerics: Burst compiles plain sequential structs of floats
    // directly, and these stay identical between dotnet (T1) and Unity/Burst.
    // Burst-safe: no allocations, no exceptions, System.Math only.

    [StructLayout(LayoutKind.Sequential)]
    public struct Float2
    {
        public float x;
        public float y;

        public Float2(float x, float y) { this.x = x; this.y = y; }

        public static readonly Float2 Zero = default;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator +(Float2 a, Float2 b) => new Float2(a.x + b.x, a.y + b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator -(Float2 a, Float2 b) => new Float2(a.x - b.x, a.y - b.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator -(Float2 a) => new Float2(-a.x, -a.y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(Float2 a, float s) => new Float2(a.x * s, a.y * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 operator *(float s, Float2 a) => new Float2(a.x * s, a.y * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Float2 a, Float2 b) => a.x * b.x + a.y * b.y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSq(Float2 a) => Dot(a, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(Float2 a) => (float)System.Math.Sqrt(Dot(a, a));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float2 Lerp(Float2 a, Float2 b, float t) => a + (b - a) * t;

        public override string ToString() => $"({x}, {y})";
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Float3
    {
        public float x;
        public float y;
        public float z;

        public Float3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; }

        public static readonly Float3 Zero = default;
        public static readonly Float3 Up = new Float3(0f, 1f, 0f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator +(Float3 a, Float3 b) => new Float3(a.x + b.x, a.y + b.y, a.z + b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator -(Float3 a, Float3 b) => new Float3(a.x - b.x, a.y - b.y, a.z - b.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator -(Float3 a) => new Float3(-a.x, -a.y, -a.z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(Float3 a, float s) => new Float3(a.x * s, a.y * s, a.z * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 operator *(float s, Float3 a) => new Float3(a.x * s, a.y * s, a.z * s);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Float3 a, Float3 b) => a.x * b.x + a.y * b.y + a.z * b.z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float LengthSq(Float3 a) => Dot(a, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Length(Float3 a) => (float)System.Math.Sqrt(Dot(a, a));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Float3 Lerp(Float3 a, Float3 b, float t) => a + (b - a) * t;

        public override string ToString() => $"({x}, {y}, {z})";
    }
}
