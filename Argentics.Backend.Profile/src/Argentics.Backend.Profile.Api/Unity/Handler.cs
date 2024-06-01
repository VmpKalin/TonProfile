using System.Text;
using Newtonsoft.Json;

namespace Argentics.Backend.Profile.Api.Unity
{
    public class Handler
    {
        public async Task AddOrUpdateProfileAsync(UserInfo userInfo)
        {
            try
            {
                var url = "http://ec2-18-157-169-245.eu-central-1.compute.amazonaws.com:7569/profile/Profiles?userId="+userInfo.Id;

                string jsonData = JsonConvert.SerializeObject(userInfo);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }
        }

        public async Task<UserInfo> GetProfileAsync(string userId)
        {
            try
            {
                var url = "http://ec2-18-157-169-245.eu-central-1.compute.amazonaws.com:7569/profile/Profiles?userId=" + userId;

                using (var httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(url);

                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(json))
                    {
                        return null;
                    }

                    return JsonConvert.DeserializeObject<UserInfo>(json);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                throw;
            }
        }
    }
}
