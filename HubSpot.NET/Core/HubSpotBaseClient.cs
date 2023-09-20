using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text.Json;
using HubSpot.NET.Api;
using HubSpot.NET.Api.Contact.Dto;
using HubSpot.NET.Core.Interfaces;
using HubSpot.NET.Core.OAuth.Dto;
using HubSpot.NET.Core.Requests;
using HubSpot.NET.Core.Serializers;
using Newtonsoft.Json;
using RestSharp;

namespace HubSpot.NET.Core
{
    public class HubSpotBaseClient : IHubSpotClient
    {
        protected readonly RequestSerializer _serializer = new RequestSerializer(new RequestDataConverter());
        private RestClient _client;

        public static string BaseUrl { get => "https://api.hubapi.com"; }

        protected readonly HubSpotAuthenticationMode _mode;

        // Used for HAPIKEY method
        protected readonly string _apiKeyName = "hapikey";
        protected readonly string _apiKey;

        // Used for OAUTH
        private HubSpotToken _token;

        protected virtual void Initialise()
        {
            _client = new RestClient(BaseUrl);
        }

        /// <summary>
        /// Creates a HubSpot client with the authentication scheme HAPIKEY.
        /// </summary>
        public HubSpotBaseClient(string apiKey)
        {
            _apiKey = apiKey;
            _mode = HubSpotAuthenticationMode.HAPIKEY;
            Initialise();
        }

        /// <summary>
        /// Creates a HubSpot client with the authentication scheme OAUTH.
        /// </summary>
        public HubSpotBaseClient(HubSpotToken token)
        {
            _token = token;
            _mode = HubSpotAuthenticationMode.OAUTH;
            Initialise();
        }

        public T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.Get, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            return Execute<T>(absoluteUriPath, entity, method, convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }
        
        public T Execute<T>(string absoluteUriPath, object entity = null, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            var json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, serialisationType);
            
            var data = SendRequest(absoluteUriPath, method, json, JsonConvert.DeserializeObject<T>);

            return data;
        }

        public T Execute<T>(string absoluteUriPath, Method method = Method.Get, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            return Execute<T>(absoluteUriPath, method, convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);

        }
        public T Execute<T>(string absoluteUriPath, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            var data = SendRequest(absoluteUriPath, method, null, JsonConvert.DeserializeObject<T>);
            return data;
        }

        public void Execute(string absoluteUriPath, object entity = null, Method method = Method.Get, bool convertToPropertiesSchema = true)
        {
            Execute(absoluteUriPath, entity, method, convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }
        public void Execute(string absoluteUriPath, object entity = null, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag)
        {
            string json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, serialisationType);

            SendRequest(absoluteUriPath, method, json);
        }
        
        public T ExecuteBatch<T>(string absoluteUriPath, object entities, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            var json = (method == Method.Get || entities == null) // TODO - this might not be all that useful.
                ? null
                : _serializer.SerializeEntity(entities, serialisationType);
            
            var data = SendRequest(absoluteUriPath, method, json, JsonConvert.DeserializeObject<T>);
            return data;
        }

        public T ExecuteMultipart<T>(string absoluteUriPath, byte[] data, string filename, Dictionary<string,string> parameters, Method method = Method.Post) where T : new()
        {
            string path = $"{BaseUrl}{absoluteUriPath}";
            RestRequest request = ConfigureRequestAuthentication(path, method);

            request.AddFile(filename, data, filename);

            foreach (KeyValuePair<string, string> kvp in parameters)
                request.AddParameter(kvp.Key, kvp.Value);

            RestResponse<T> response = _client.Execute<T>(request);

            T responseData = response.Data;

            if (!response.IsSuccessful)
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription));

            return responseData;
        }

        public T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.Get, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            return ExecuteList<T>(absoluteUriPath, entity, method, convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }
        public T ExecuteList<T>(string absoluteUriPath, object entity = null, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            var json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, true);
            
            var data = SendRequest(absoluteUriPath, method, json, JsonConvert.DeserializeObject<T>);

            return data;
        }

        protected virtual T SendRequest<T>(string path, Method method, string json, Func<string, T> deserializeFunc) where T : IHubSpotModel, new()
        {
            var responseData = SendRequest(path, method, json);
            return string.IsNullOrWhiteSpace(responseData) ? default : deserializeFunc(responseData);
        }

        protected virtual string SendRequest(string path, Method method, string json)
        {
            var request = ConfigureRequestAuthentication(path, method);

            if (method != Method.Get && !string.IsNullOrWhiteSpace(json))
                request.AddParameter("application/json", json, ParameterType.RequestBody);

            var response = _client.Execute(request);

            var responseData = response.Content;
            // TODO - remove debugging
            //Console.WriteLine($"Inbound response");
            //Console.WriteLine(responseData);
            if (!response.IsSuccessful)
                throw new HubSpotException("Error from HubSpot", new HubSpotError(response.StatusCode, response.StatusDescription), responseData);

            return responseData;
        }

        /// <summary>
        /// Configures a <see cref="RestRequest"/> based on the authentication scheme detected and configures the endpoint path relative to the base path.
        /// </summary>
        protected virtual RestRequest ConfigureRequestAuthentication(string path, Method method)
        {
#if NET451
            RestRequest request = new RestRequest(path, method);
            request.RequestFormat = DataFormat.Json;
#else
            //RestRequest request = new RestRequest(path, method, DataFormat.Json);
            RestRequest request = new RestRequest(path, method);
#endif
            switch (_mode)
            {
                case HubSpotAuthenticationMode.OAUTH:
                    request.AddHeader("Authorization", GetAuthHeader(_token));
                    break;
                default:
                    request.AddQueryParameter(_apiKeyName, _apiKey);
                    break;
            }

            // CEH https://restsharp.dev/serialization.html#json
            //request.JsonSerializer = new NewtonsoftRestSharpSerializer();
            
            return request;
        }

        protected virtual string GetAuthHeader(HubSpotToken token) => $"Bearer {token.AccessToken}";


        /// <summary>
        /// Updates the OAuth token used by the client.
        /// </summary>
        public void UpdateToken(HubSpotToken token) => _token = token;
    }
}