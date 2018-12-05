using UnityEngine;

public struct Int3
{
    public int x,y,z;

    public Int3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public int ManhattanDistance(Int3 b)
    {
        return Mathf.Abs(this.x - b.x) +
               Mathf.Abs(this.y - b.y) +
               Mathf.Abs(this.z - b.z);
    }

    public static Int3 operator%(Int3 a, int b) { return new Int3(a.x % b, a.y % b, a.z % b); }
    public static Int3 operator%(Int3 a, Int3 b) { return new Int3(a.x % b.x, a.y % b.y, a.z % b.z); }

    public static Int3 operator+(Int3 a, Int3 b) { return new Int3(a.x + b.x, a.y + b.y, a.z + b.z); }
        // Subtracts one vector from another.
    public static Int3 operator-(Int3 a, Int3 b) { return new Int3(a.x - b.x, a.y - b.y, a.z - b.z); }

    public static bool operator==(Int3 a, Int3 b) { return  a.x == b.x && a.y == b.y && a.z == b.z; }
    public static bool operator!=(Int3 a, Int3 b) { return  !(a.x == b.x && a.y == b.y && a.z == b.z); }

    public static Int3 operator*(Int3 a, int b) { return new Int3(a.x * b, a.y * b, a.z * b); }
    public static Int3 operator*(int b, Int3 a) { return new Int3(a.x * b, a.y * b, a.z * b); }

    public static Int3 one() { return new Int3(1,1,1); }
    public static Int3 zero() { return new Int3(0,0,0); }

    public static Int3 random(Int3 a, Int3 b) {
        Int3 int3Value = new Int3(0,0,0);

        int3Value.x = Random.Range(a.x, b.x);
        int3Value.y = Random.Range(a.y, b.y);
        int3Value.z = Random.Range(a.z, b.z);

        return int3Value;
    }
}