using System;
using System.IdentityModel.Tokens;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Core
{
	/// <summary>
	/// This class maintains an instance of base64 string and returns decoded string as needed.
	/// </summary>
	internal sealed class Base64String
	{
		/// <summary>
		/// Creates an instance of class.
		/// </summary>
		/// <param name="base64String">input st</param>
		public Base64String(string base64String)
		{
			myBase64String = base64String;
		}

		/// <summary>
		/// converts encoded base64 string to plain text
		/// </summary>
		/// <returns>decoded plain text string</returns>
		public override string ToString()
		{
			return Base64UrlEncoder.Decode(myBase64String.Reverse());
		}

		public override bool Equals(object obj)
		{
			return myBase64String.Equals(((Base64String)obj)?.ToString());
		}

		public override int GetHashCode()
		{
			return (myBase64String != null ? myBase64String.GetHashCode() : 0);
		}

		private readonly string myBase64String;
	}

	/// <summary>
	/// Extension class that contain string extension helpers
	/// </summary>
	internal static class StringExtensions
	{
		public static string Reverse(this string s)
		{
			char[] charArray = s.ToCharArray();
			Array.Reverse(charArray);
			return new string(charArray);
		}
	}
}
