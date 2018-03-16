using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Common
{
    class Extensions
    {
        /// <summary>
        /// Performs a deep copy of the source object's public members
        /// 
        /// Not the most performant option but will work well for our
        ///    purposes and doesn't require implementing ICloneable / copy constructor
        ///    with constant updates upon adding new fields.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns>Clone of *public* properties of T</returns>
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source, new Newtonsoft.Json.Converters.StringEnumConverter());
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}
