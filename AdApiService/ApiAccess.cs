using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ApiSecurity
{
    public class ApiAccess
    {

        /// <summary>
        /// Used by IsAllowed to determine if the specified item matches or exists in the 
        /// value associated with the apikey in the web.config file. 
        /// </summary>
        /// <param name="access">
        /// This is the value of the web.config appsettings key associated with the apikey
        /// associated with the current request. It can be a single item that must match
        /// exactly or a comma separated list of items, any of which can be a match, i.e., 
        /// first match returns true.
        /// </param>
        /// <param name="item">The item to test.</param>
        /// <returns>True if the item matches or exists in the access string, false otherwise.</returns>
        public static Boolean AccessContains(string access, string item)
        {
            if (!(string.IsNullOrWhiteSpace(access) || string.IsNullOrWhiteSpace(item)))
            {
                if (access.Contains(',')) // if access is a comma separated list
                {
                    // split the list into strings
                    List<string> accessItems = access.Split(new char[] { ',' }).ToList();
                    // using foreach to use string.equals
                    foreach (string accessItem in accessItems)
                    {
                        if (string.Equals(item.Trim(), accessItem.Trim(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                else // else the access string might be a single item
                {
                    // At this point the access string can be either an asterix, meaning all access, or a single item that 
                    // should match the value of the item parameter.
                    return string.Equals(access.Trim(), item.Trim(), StringComparison.CurrentCultureIgnoreCase); // access should be an asterix
                }
            }
            // if we get to here the item and/or the access string weren't expected
            return false;
        }

        /// <summary>
        /// This method verifies the apikey is valid. The item parameter is optional. If not provided,
        /// it is the name of the caller. The value of the key associated with the specified API key
        /// (as found in the AppSettings section of the web.config file) is tested to see if it is
        /// an asterix (meaning all access), the name of the calling method, a comma separated list of
        /// items that contains either the name of the calling method or the value of the item parameter.
        /// 
        /// </summary>
        /// <param name="apikey">
        /// The key used to authorize the request. It is the name of a key in the AppSettings section of
        /// the Web.config file. The value associated with the key is either an asterix, a comma separated
        /// list of items which can be methods, HTTP methods, or any other string an application needs to 
        /// use as an access identifier.
        /// </param>
        /// <returns>True if the apikey is valid, false otherwise.</returns>
        public static Boolean IsAllowed(string apikey, [System.Runtime.CompilerServices.CallerMemberName] string item = "")
        {

            Boolean allowed = false;
            string allAccess = "*";
            // determine the httpmethod type (GET, PUT, POST, DELETE...)
            HttpContext context = HttpContext.Current;
            string httpMethod = context.Request.HttpMethod;
            // get the value of the key from the web.config
            string access = ConfigurationManager.AppSettings.Get(apikey).ToUpper();
            // ApiLogging.EventLogger.Instance.WriteInfo(string.Format("IsAllowed:\napikey: {0}\nitem: {1}\naccess: {2}", apikey, item, access));

            // if the key exists a value would be returned - test it for level of access
            if (
                    (access != null) &&
                    (
                        AccessContains(access, allAccess) ||
                        AccessContains(access, item) ||
                        AccessContains(access, httpMethod)
                    )
                )
            {
                allowed = true;
            }
            return allowed;
        }

    } // end class ApiAccess

} // end namespace