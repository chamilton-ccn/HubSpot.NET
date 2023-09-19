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
            
            //T data = SendRequest(absoluteUriPath, method, json, responseData => (T)_serializer.DeserializeEntity<T>(responseData, serialisationType != SerialisationType.Raw));
            // TODO - experimental. Attempting to make serialization/deserialization less complex. Original line is above.
            var data = SendRequest(absoluteUriPath, method, json, JsonConvert.DeserializeObject<T>);

            return data;
        }

        public T Execute<T>(string absoluteUriPath, Method method = Method.Get, bool convertToPropertiesSchema = true) where T : IHubSpotModel, new()
        {
            return Execute<T>(absoluteUriPath, method, convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);

        }
        public T Execute<T>(string absoluteUriPath, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            //T data = SendRequest(absoluteUriPath, method, null, responseData => (T)_serializer.DeserializeEntity<T>(responseData, serialisationType != SerialisationType.Raw));
            // TODO - experimental. Attempting to make serialization/deserialization less complex. Original line is above.
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

        /*public void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.Get, bool convertToPropertiesSchema = true)
        {
            ExecuteBatch(absoluteUriPath, entities, method, convertToPropertiesSchema ? SerialisationType.PropertiesSchema : SerialisationType.Raw);
        }*/
        
        //public void ExecuteBatch(string absoluteUriPath, List<object> entities, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag)
        //public T ExecuteBatch<T>(string absoluteUriPath, List<object> entities, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        //public T ExecuteBatch<T>(string absoluteUriPath, object entities, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        //public T ExecuteBatch<T>(string absoluteUriPath, object entities, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        //public T ExecuteBatch<T>(string absoluteUriPath, T entities, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag)
        public T ExecuteBatch<T>(string absoluteUriPath, object entities, Method method = Method.Get, SerialisationType serialisationType = SerialisationType.PropertyBag) where T : IHubSpotModel, new()
        {
            string json = (method == Method.Get || entities == null)
                ? null
                : _serializer.SerializeEntity(entities, serialisationType);
            
            // TODO - remove debugging
            // It makes sense for "results" to be empty here
            Console.WriteLine($"BEGIN HubSpotBaseClient line #112");
            Console.WriteLine(json);
            Console.WriteLine($"END HubSpotBaseClient line #112");
            
            /*var data = SendRequest(absoluteUriPath,
                method,
                json,
                responseData => (T)_serializer.DeserializeListEntity<T>(responseData, serialisationType != SerialisationType.Raw));*/
            // TODO - remove debugging
            var testJson = JsonConvert.DeserializeObject<T>(json);
            foreach (var i in testJson.GetType().GetProperties())
            {
                if (i.Name == "Results")
                {
                    Console.WriteLine(i.GetValue(testJson));
                    foreach (var j in (List<ContactHubSpotModel>)i.GetValue(testJson))
                    {
                        Console.WriteLine($"DOES IT FUCKING WORK BEFORE SendRequest? {j.Email}");
                    }
                    
                }
                    
            }
            
            // TODO - experimental. Attempting to make serialization/deserialization less complex. Original line is above.
            var data = SendRequest(absoluteUriPath, method, json, JsonConvert.DeserializeObject<T>);

            Console.WriteLine($"HubSpotBaseClient line #123");
            foreach (var property in data.GetType().GetProperties())
            {
                //Console.WriteLine($"-> DATA PROPERTY: {property.Name}");
                if (property.Name == "Results")
                {
                    Console.WriteLine(property.GetValue(data));
                    foreach (var j in (List<ContactHubSpotModel>)property.GetValue(data))
                    {
                        Console.WriteLine($"DOES IT FUCKING WORK AFTER SendRequest? {j.Email}");
                    }
                }                
            }
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
            string json = (method == Method.Get || entity == null)
                ? null
                : _serializer.SerializeEntity(entity, true);
            // TODO - remove
            Console.WriteLine($"-> HubSpotBaseClient line #146");
            Console.WriteLine(json);
            
            /*var data = SendRequest(
                absoluteUriPath,
                method,
                json,
                responseData => (T)_serializer.DeserializeListEntity<T>(responseData, serialisationType != SerialisationType.Raw));*/
            
            // TODO - experimental. Attempting to make serialization/deserialization less complex. Original line is above.
            var data = SendRequest(
                absoluteUriPath,
                method,
                json,
                JsonConvert.DeserializeObject<T>);
            // TODO - remove
            Console.WriteLine($"-> HubSpotBaseClient line #162");
            Console.WriteLine(data);
            return data;
        }

        protected virtual T SendRequest<T>(string path, Method method, string json, Func<string, T> deserializeFunc) where T : IHubSpotModel, new()
        {
            string responseData = SendRequest(path, method, json);
            
            //TODO - remove debugging
            Console.WriteLine($"***** SENDING REQUEST from HubSpotBaseClient line 195");
            
            if (string.IsNullOrWhiteSpace(responseData))
                return default;

            return deserializeFunc(responseData);
        }

        protected virtual string SendRequest(string path, Method method, string json)
        {
            //TODO - remove debugging
            Console.WriteLine($"***** SENDING REQUEST from HubSpotBaseClient line 206");
            
            RestRequest request = ConfigureRequestAuthentication(path, method);

            if (method != Method.Get && !string.IsNullOrWhiteSpace(json))
                request.AddParameter("application/json", json, ParameterType.RequestBody);

            RestResponse response = _client.Execute(request);

            string responseData = response.Content;
            
            //TODO - remove debugging
            Console.WriteLine($"***** RESPONSE");
            Console.WriteLine(responseData);
            var testJson = JsonConvert.DeserializeObject<ContactListHubSpotModel<ContactHubSpotModel>>(responseData);
            Console.WriteLine($" #### >>>> #### Status is here? {testJson.Status}");
            foreach (var result in testJson.Results)
            {
                Console.WriteLine($"FUCKING RESULTS! {result.Email}");
            }

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