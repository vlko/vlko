using Microsoft.AspNetCore.Routing;

namespace vlko.core.web.Base
{
    public class DynamicRoutesResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicRoutesResult"/> class.
        /// </summary>
        public DynamicRoutesResult()
        {
            RouteValues = new RouteValueDictionary();
        }

        /// <summary>
        /// Gets the route values.
        /// </summary>
        public RouteValueDictionary RouteValues { get; private set; }

        /// <summary>
        /// Gets or sets the name of the route.
        /// </summary>
        /// <value>
        /// The name of the route.
        /// </value>
        public string RouteName { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// Adds the specified key with value to RouteValues.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void Add(string key, object value)
        {
            RouteValues.Add(key, value);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Url;
        }

        /// <summary>
        /// Implicit conversion to string
        /// </summary>
        public static implicit operator string(DynamicRoutesResult result)
        {
            return result.Url;
        }
    }
}