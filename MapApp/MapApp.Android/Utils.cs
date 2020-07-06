using Android.Content;
using Com.Mapbox.Mapboxsdk;
using Java.Lang;

namespace MapApp.Droid
{
   public class Utils
    {
        public static string GetMapboxAccessToken(Context context)
        {
            try
            {
                // Read out AndroidManifest
                string token = Mapbox.AccessToken;
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new IllegalArgumentException();
                }
                return token;
            }
            catch (System.Exception)
            {
                // Use fallback on string resource, used for development
                int tokenResId = context.Resources
                                        .GetIdentifier("mapbox_access_token", "string", context.PackageName);
                return tokenResId != 0 ? context.GetString(tokenResId) : null;
            }
        }
    }
}