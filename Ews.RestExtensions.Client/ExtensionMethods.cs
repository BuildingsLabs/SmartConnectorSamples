using System.Web;

namespace Ews.RestExtensions.Client
{
    public static class ExtensionMethods
    {
        #region UrlEncodeToString
        public static string UrlEncodeToString(this string stringToEncode)
        {
            return string.IsNullOrEmpty(stringToEncode) ? null : HttpUtility.UrlEncode(stringToEncode);
        }
        #endregion
    }
}
