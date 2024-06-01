using Newtonsoft.Json;

namespace Argentics.Backend.Profile.Core.Tools
{
    public static class JsonConvertExt
    {
        public static T TryDeserialize<T>(this string obj) where T : class 
        {
			bool isFailed = false;
            T value = default(T);

			try
			{
				value = JsonConvert.DeserializeObject<T>(obj);	
			}
            finally { isFailed = true; }
			
            return value;
        }
    }
}
