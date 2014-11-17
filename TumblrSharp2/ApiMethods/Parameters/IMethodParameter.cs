namespace TumblrSharp2.ApiMethods.Parameters
{
    using System;
    using System.Net.Http;
    using TumblrSharp2.ApiMethods;

    /// <summary>
	/// Represents a parameter for an <see cref="ApiMethod"/>.
	/// </summary>
	public interface IMethodParameter : IEquatable<IMethodParameter>
	{
		/// <summary>
		/// Gets the parameter name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Converts the parameter to a <see cref="HttpContent"/>.
		/// </summary>
		/// <returns>
		/// The parameter as a <see cref="HttpContent"/>.
		/// </returns>
		HttpContent AsHttpContent();
	}
}
