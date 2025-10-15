using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace IFI.Integrations.Objects.JsonProxyClient
{
    public interface IProxyClient : IDisposable
    {
        HttpResponseMessage Delete(string requestUrl);
        T Deserialize<T>(HttpResponseMessage response);
        T Deserialize<T>(HttpResponseMessage response, JsonSerializerSettings deSerializerSettings);
        void SetBaseUrl(string baseUrl);
        string EncodeParameter(string parm);
        HttpResponseMessage Get(string requestUrl);
        string GetResponsePayload(HttpResponseMessage response);
        HttpResponseMessage Post(string requestUrl, object payload, bool serializeToCamelCase = true);
        HttpResponseMessage PostJsonText(string requestUrl, string payload);
        HttpResponseMessage Put(string requestUrl, object payload, bool serializeToCamelCase = true);
        string Serialize(object payload, bool serializeToCamelCase = true);
    }

    public class ProxyClient : IProxyClient
    {
        protected readonly HttpClient Client = new HttpClient();

        readonly JsonSerializerSettings CamelCaseFormatter = new JsonSerializerSettings();

        public ProxyClient(string baseUrl)
        {
            Client.BaseAddress = new Uri(baseUrl);
            Init();
        }

        public ProxyClient()
        {
            Init();
        }

        private void Init()
        {
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            CamelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            SetServicePointManager();
        }

        private void SetServicePointManager()
        {
            if ((int)ServicePointManager.SecurityProtocol < 3072) //Set SecurityProtocol to TLs12 if it is lower. Anything higher should be fine.
            {
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            }
        }

        public void SetBaseUrl(string baseUrl)
        {
            this.Client.BaseAddress = new Uri(baseUrl);
        }

        public void AddHeader(string Key, string Value)
        {
            this.Client.DefaultRequestHeaders.Add(Key, Value);
        }

        public HttpResponseMessage Get(string requestUrl)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            return Task.Run(() => Client.GetAsync(requestUrl)).Result;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUrl)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            return await Client.GetAsync(requestUrl);
        }

        public bool GetAndDeserializeResponseText<T>(string requestUrl, out HttpResponseMessage response, out T responseText)
        {
            response = Get(requestUrl);
            return DeserializeResponse<T>(response, out responseText);
        }

        public HttpResponseMessage Delete(string requestUrl)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            return Task.Run(() => Client.DeleteAsync(requestUrl)).Result;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUrl)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            return await Client.DeleteAsync(requestUrl);
        }

        public bool DeleteAndDeserializeResponseText<T>(string requestUrl, object payload, out HttpResponseMessage response, out T responseText)
        {
            response = Delete(requestUrl);
            return DeserializeResponse<T>(response, out responseText);
        }

        public HttpResponseMessage Post(string requestUrl, object payload, bool serializeToCamelCase = true)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            string pltext = string.Empty;
            if (serializeToCamelCase)
                pltext = JsonConvert.SerializeObject(payload, CamelCaseFormatter);
            else
                pltext = JsonConvert.SerializeObject(payload);

            return Task.Run(() => Client.PostAsync(requestUrl, new StringContent(pltext, Encoding.UTF8, "application/json"))).Result;
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUrl, object payload, bool serializeToCamelCase = true)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            string pltext = string.Empty;
            if (serializeToCamelCase)
                pltext = JsonConvert.SerializeObject(payload, CamelCaseFormatter);
            else
                pltext = JsonConvert.SerializeObject(payload);

            return await Client.PostAsync(requestUrl, new StringContent(pltext, Encoding.UTF8, "application/json"));
        }

        public bool PostAndDeserializeResponseText<T>(string requestUrl, object payload, out HttpResponseMessage response, out T responseText, bool serializeToCamelCase = true)
        {
            response = Post(requestUrl, payload, serializeToCamelCase);
            return DeserializeResponse<T>(response, out responseText);
        }

        public HttpResponseMessage PostJsonText(string requestUrl, string payload)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            return Task.Run(() => Client.PostAsync(requestUrl, new StringContent(payload, Encoding.UTF8, "application/json"))).Result;
        }

        public bool PostJsonTextAndDeserializeResponseText<T>(string requestUrl, string payload, out HttpResponseMessage response, out T responseText)
        {
            response = PostJsonText(requestUrl, payload);
            return DeserializeResponse<T>(response, out responseText);
        }

        public HttpResponseMessage Put(string requestUrl, object payload, bool serializeToCamelCase = true)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            string pltext = string.Empty;
            if (serializeToCamelCase)
                pltext = JsonConvert.SerializeObject(payload, CamelCaseFormatter);
            else
                pltext = JsonConvert.SerializeObject(payload);

            return Task.Run(() => Client.PutAsync(requestUrl, new StringContent(pltext, Encoding.UTF8, "application/json"))).Result;
        }

        public async Task<HttpResponseMessage> PutAsync(string requestUrl, object payload, bool serializeToCamelCase = true)
        {
            if (this.Client.BaseAddress == null)
                throw new Exception("Baseurl has not been set.");
            string pltext = string.Empty;
            if (serializeToCamelCase)
                pltext = JsonConvert.SerializeObject(payload, CamelCaseFormatter);
            else
                pltext = JsonConvert.SerializeObject(payload);

            return await Client.PutAsync(requestUrl, new StringContent(pltext, Encoding.UTF8, "application/json"));
        }

        public bool PutAndDeserializeResponseText<T>(string requestUrl, object payload, out HttpResponseMessage response, out T responseText, bool serializeToCamelCase = true)
        {
            response = Put(requestUrl, payload, serializeToCamelCase);
            return DeserializeResponse<T>(response, out responseText);
        }

        public string Serialize(object payload, bool serializeToCamelCase = true)
        {
            if (serializeToCamelCase)
                return JsonConvert.SerializeObject(payload, CamelCaseFormatter);
            else
                return JsonConvert.SerializeObject(payload);
        }

        public string GetResponsePayload(HttpResponseMessage response)
        {
            return Task.Run(() => response.Content.ReadAsStringAsync()).Result;
        }

        private bool DeserializeResponse<T>(HttpResponseMessage response, out T responseText)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                responseText = Deserialize<T>(response);
                return true;
            }
            else
            {
                responseText = default;
                return false;
            }
        }

        public T Deserialize<T>(HttpResponseMessage response)
        {
            var responseText = GetResponsePayload(response);
            return JsonConvert.DeserializeObject<T>(responseText);
        }
        public T Deserialize<T>(HttpResponseMessage response, JsonSerializerSettings deSerializerSettings)
        {
            var responseText = GetResponsePayload(response);
            return JsonConvert.DeserializeObject<T>(responseText, deSerializerSettings);
        }

        public string EncodeParameter(string parm)
        {
            return HttpUtility.UrlEncode(parm);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Client.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

