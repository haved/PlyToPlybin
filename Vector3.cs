/*
Copyright (c) 2006 - 2008 The Open Toolkit library.
Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */

namespace PlyToPlybin {
	public struct Vector3 {
		public float X,Y,Z;
		public Vector3(float x, float y, float z) {
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// /// Defines a unit-length Vector3 that points towards the Z-axis.
		/// </summary>
		public static readonly Vector3 UnitZ = new Vector3(0, 0, 1);

		/// <summary>
		/// Gets the length (magnitude) of the vector.
		/// </summary>
		/// <see cref="LengthFast"/>
		/// <seealso cref="LengthSquared"/>
		public float Length
		{
		    get
		    {
		        return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
		    }
		}

		/// <summary>
		/// Caclulate the cross (vector) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The cross product of the two inputs</returns>
		public static Vector3 Cross(Vector3 left, Vector3 right)
		{
		    Vector3 result;
		    Cross(ref left, ref right, out result);
		    return result;
		}

		/// <summary>
		/// Caclulate the cross (vector) product of two vectors
		/// </summary>
		/// <param name="left">First operand</param>
		/// <param name="right">Second operand</param>
		/// <returns>The cross product of the two inputs</returns>
		/// <param name="result">The cross product of the two inputs</param>
		public static void Cross(ref Vector3 left, ref Vector3 right, out Vector3 result)
		{
		    result = new Vector3(left.Y * right.Z - left.Z * right.Y,
		        left.Z * right.X - left.X * right.Z,
		        left.X * right.Y - left.Y * right.X);
		}

		/// <summary>
		/// Adds two instances.
		/// </summary>
		/// <param name="left">The first instance.</param>
		/// <param name="right">The second instance.</param>
		/// <returns>The result of the calculation.</returns>
		public static Vector3 operator +(Vector3 left, Vector3 right)
		{
		    left.X += right.X;
		    left.Y += right.Y;
		    left.Z += right.Z;
		    return left;
		}


		/// <summary>
		/// Returns a copy of the Vector3 scaled to unit length.
		/// </summary>
		public Vector3 Normalized()
		{
		    Vector3 v = this;
		    v.Normalize();
		    return v;
		}

		/// <summary>
		/// Scales the Vector3 to unit length.
		/// </summary>
		public void Normalize()
		{
		    float scale = 1.0f / this.Length;
		    X *= scale;
		    Y *= scale;
		    Z *= scale;
		}
	}
}
