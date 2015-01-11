// Author : Nikola Stepan
// E-mail : nikola.stepan@vz.htnet.hr
// Web    : http://calcsharp.net

// fractions are auto-reducing and immutable
// numerator and denominator must be in [-int.MaxValue, int.MaxValue]
// binary operator overloading methods can throw overflow exception
using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

[Serializable]
public struct Fraction : IComparable<Fraction>
{   
	//top/bottom
	public readonly int num;
	public readonly int den;

	#region Constructors	

	public Fraction (int num)
	{
		this.num=num;
		this.den=1;
	}

	public Fraction (int num, int den)
	{	
		//using (Benchmark.Start("GCD"))
		{
			if (den>10000)
			{
				int gcd = GCD ( Math.Abs( num ), Math.Abs( den ) );

				num = ( num / gcd );
				den = ( den / gcd );
			}
		}

		CheckDenominatorZero(den);		

		this.num=num;
		this.den=den;
	}

	#endregion

	#region Overloaded Binary Operators ( +-*/^ )

	#region Add

	public static Fraction operator + (Fraction a, Fraction b)
	{
		int r1 = a.num * b.den + b.num * a.den;
		int r2 = a.den * b.den;
		return new Fraction(r1, r2);
	}

	public static Fraction operator + (Fraction a, int b)
	{
		return new Fraction(b*a.den + a.num,a.den);
	}

	public static Fraction operator + (int a, Fraction b)
	{
		return new Fraction(a*b.den + b.num,b.den);
	}

	#endregion

	#region Substract

	public static Fraction operator - (Fraction a, Fraction b)
	{
		int r1 = a.num * b.den - b.num * a.den;
		int r2 = a.den * b.den;
		return new Fraction(r1, r2);
	}

	public static Fraction operator - (Fraction a, int b)
	{
		return new Fraction(a.num-b*a.den,a.den);
	}

	public static Fraction operator - (int a, Fraction b)
	{
		return new Fraction(a*b.den-b.num,b.den);
	}

	#endregion

	#region Multiply

	public static Fraction operator * (Fraction a, Fraction b)
	{
		int r1 = a.num * b.num;
		int r2 = a.den * b.den;
		return new Fraction(r1, r2);
	}

	public static Fraction operator * (Fraction a, int b)
	{
		return new Fraction(a.num*b,a.den);
	}

	public static Fraction operator * (int a, Fraction b)
	{
		return new Fraction(a*b.num,b.den);
	}

	#endregion

	#region Divide

	public static Fraction operator / (Fraction a, Fraction b)
	{
		int r1 = a.num * b.den;
		int r2 = a.den * b.num;

		if (r2 == 0)
		{
			throw new DivideByZeroException();
		}
		else
		{
			return new Fraction(r1, r2);
		}
	}

	public static Fraction operator / (Fraction a, int b)
	{
		return new Fraction(a.num,a.den*b);
	}

	public static Fraction operator / (int a, Fraction b)
	{
		return new Fraction(a*b.den,b.num);
	}

	#endregion

	#region Power

	public static Fraction operator ^ (Fraction a, int n)
	{
		return new Fraction((int)Math.Pow(a.num, n), (int)Math.Pow(a.den, n));
	}

	#endregion

	#endregion

	#region Comparison operators

	public static bool operator == (Fraction a, Fraction b)
	{
		return a.num * b.den == b.num * a.den;
	}

	public static bool operator == (Fraction a, int b)
	{
		return a.num == b * a.den;
	}

	public static bool operator == (int b, Fraction a)
	{
		return a.num == b * a.den;
	}

	public static bool operator != (Fraction a, Fraction b)
	{
		return a.num * b.den != b.num * a.den;
	}	

	public static bool operator != (int a, Fraction b)
	{
		return (!(a == b));
	}	

	public static bool operator != (Fraction a, int b)
	{
		return (!(a == b));
	}	

	public static bool operator > (Fraction a, int b)
	{
		return a.num > b * a.den;
	}

	public static bool operator > (int a, Fraction b)
	{
		return a * b.den > b.num;
	}

	public static bool operator > (Fraction a, Fraction b)
	{
		return a.num * b.den > b.num * a.den;
	}

	public static bool operator >= (int a, Fraction b)
	{
		return (!(a < b));
	}

	public static bool operator >= (Fraction a, int b)
	{
		return (!(a < b));
	}

	public static bool operator >= (Fraction a, Fraction b)
	{
		return (!(a < b));
	}

	public static bool operator < (int a, Fraction b)
	{
		return a * b.den < b.num;
	}

	public static bool operator < (Fraction a, int b)
	{
		return a.num < b * a.den;
	}

	public static bool operator < (Fraction a, Fraction b)
	{
		return a.num * b.den < b.num * a.den;
	}

	public static bool operator <= (Fraction a, Fraction b)
	{
		return (!(a > b));
	}

	public static bool operator <= (int a, Fraction b)
	{
		return (!(a > b));
	}

	public static bool operator <= (Fraction a, int b)
	{
		return (!(a > b));
	}

	#endregion

	#region Interfaces


	public int CompareTo(Fraction other)
	{
		return ((float)this).CompareTo((float)other);
	}

	#endregion

	#region Misc

	public override string ToString ()
	{
		if (this.den == 1)
		{
			return this.num.ToString();
		}
		else
		{
			return this.num + "/" + this.den;
		}
	}

	public override bool Equals (object o)
	{
		if (o == null || o.GetType() != GetType())
		{
			return false;
		}
		Fraction f = (Fraction)o;
		return (this == f);
	}		

	public override int GetHashCode ()
	{
		return (this.num ^ this.den);
	}

	public static explicit operator double (Fraction f)
	{
		return (double)f.num / f.den;
	}

	public static explicit operator float (Fraction f)
	{
		return (float)f.num / f.den;
	}

	public static Fraction zero = new Fraction(0,1);
	public static implicit operator Fraction(int value) 
	{
		// Note that because RomanNumeral is declared as a struct, 
		// calling new on the struct merely calls the constructor 
		// rather than allocating an object on the heap:
		return new Fraction(value);
	}

	// Euclid identities

	// GCD(A,B)=GCD(B,A) 
	// GCD(A,B)=GCD(-A,B) 
	// GCD(A,0)=ABS(A) 
	// GCD(0,0)=0
	// GCD(A,B)=GCD(B,A%B)

	public static int GCD(int a, int b) 
	{
		int t;

		// Ensure B > A
		if (a > b) 
		{
			t = b;
			b = a;
			a = t;
		}

		// Find 
		while (b != 0)
		{
			t = a % b;
			a = b;
			b = t;
		}

		return a;
	}


	private static void CheckDenominatorZero (int den)
	{
		if (den == 0)
		{
			throw new ArithmeticException("The denominator of any fraction cannot have the value zero");
		}
	}

	// throws FormatException if wrong fraction format
	// throws OverflowException if reduced fraction does not fit in fraction range
	// throws ArithmeticException if denominator is zero
	public static Fraction Parse (string fraction)
	{
		if (fraction == null)
		{
			throw new FormatException();
		}

		string[] split = fraction.Split('/');
		int len = split.Length;

		if (len == 2)
		{
			int s0 = ParseUtils.IntParseFast(split [0]);
			int s1 = ParseUtils.IntParseFast(split [1]);
			return new Fraction(s0, s1);
		}
		else
			if (len == 4)
			{
				int s0 = ParseUtils.IntParseFast(split [0]);
				int s1 = ParseUtils.IntParseFast(split [1]);
				Fraction f1 = new Fraction(s0, s1);

				int s2 = ParseUtils.IntParseFast(split [2]);
				int s3 = ParseUtils.IntParseFast(split [3]);
				Fraction f2 = new Fraction(s2, s3);

				return f1 / f2;
			}
			else
			{
				throw new FormatException();
			}
	}

	public Fraction Inverse ()
	{
		return new Fraction(this.den, this.num);
	}

	#endregion
}
