﻿using System.Linq;
using HubSpot.NET.Core.Errors;

namespace HubSpot.NET.Core.OAuth
{
    using System.Collections.Generic;
    using System.Text;

    using HubSpot.NET.Core;
    using HubSpot.NET.Core.OAuth.Dto;
	using Newtonsoft.Json;
	using RestSharp;

	public class HubSpotOAuthApi
    {
        public string ClientId { get; protected set; }
        private string _clientSecret;
        private readonly string _basePath;

        public virtual string MidRoute => "oauth/v1/token";

        private readonly Dictionary<OAuthScopes, string> OAuthScopeNameConversions = new Dictionary<OAuthScopes, string>
        {
            { OAuthScopes.Automation , "automation" },
            { OAuthScopes.BusinessIntelligence, "business-intelligence" },
            { OAuthScopes.Contacts , "contacts" },
            { OAuthScopes.Content , "content" },
            { OAuthScopes.ECommerce , "e-commerce" },
            { OAuthScopes.Files , "files" },
            { OAuthScopes.Forms , "forms" },
            { OAuthScopes.HubDb , "hubdb" },
            { OAuthScopes.IntegrationSync , "integration-sync" },
            { OAuthScopes.Reports , "reports" },
            { OAuthScopes.Social , "social" },
            { OAuthScopes.Tickets , "tickets" },
            { OAuthScopes.Timeline , "timeline" },
            { OAuthScopes.TransactionalEmail , "transactional-email" }
        };


        public HubSpotOAuthApi(string basePath, string clientId, string clientSecret)
        {
            _basePath = basePath;
            ClientId = clientId;
            _clientSecret = clientSecret;
        }

        public HubSpotToken Authorize(string authCode, string redirectUri)
        {
            RequestTokenHubSpotModel model = new RequestTokenHubSpotModel()
            {
                ClientId = ClientId,
                ClientSecret = _clientSecret,
                Code = authCode,
                RedirectUri = redirectUri
            };

            HubSpotToken token = InitiateRequest(model, _basePath);
            return token;
        }

        public HubSpotToken Refresh(string redirectUri, HubSpotToken token)
        {
            RequestRefreshTokenHubSpotModel model = new RequestRefreshTokenHubSpotModel()
            {
                ClientId = ClientId,
                ClientSecret = _clientSecret,
                RedirectUri = redirectUri,
                RefreshToken = token.RefreshToken
            };

            HubSpotToken refreshToken = InitiateRequest(model, _basePath);
            return refreshToken;
        }

        public void UpdateCredentials(string id, string secret)
        {
            ClientId = id;
            _clientSecret = secret;
        }

        private HubSpotToken InitiateRequest<K>(K model, string basePath, params OAuthScopes[] scopes)
        {
            RestClient client = new RestClient(basePath);

            StringBuilder builder = new StringBuilder();
            foreach (OAuthScopes scope in scopes)
            {
                if (builder.Length == 0)
                    builder.Append($"{OAuthScopeNameConversions[scope]}");
                else
                    builder.Append($"%20{OAuthScopeNameConversions[scope]}");
            }

            RestRequest request = new RestRequest(MidRoute)
            {
                // CEH https://restsharp.dev/serialization.html#json
                //JsonSerializer = new FakeSerializer()
            };

            Dictionary<string, string> jsonPreStringPairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(model));

            StringBuilder bodyBuilder = new StringBuilder();
            foreach(KeyValuePair<string,string> pair in jsonPreStringPairs)
            {
                if (bodyBuilder.Length > 0)
                    bodyBuilder.Append("&");

                bodyBuilder.Append($"{pair.Key}={pair.Value}");
            }

            request.AddJsonBody(bodyBuilder.ToString());
            request.AddHeader("ContentType", "application/x-www-form-urlencoded");

            if (builder.Length > 0)
                request.AddQueryParameter("scope", builder.ToString());

            // CEH 
            //RestResponse<HubSpotToken> serverResponse = client.Post<HubSpotToken>(request);
            
            RestResponse serverResponse = client.Post(request);
            if (serverResponse.Content != null)
            {
                return JsonConvert.DeserializeObject<HubSpotToken>(serverResponse.Content);
            }
            
            if (serverResponse.ResponseStatus != ResponseStatus.Completed)
                throw new HubSpotException("Server did not respond to authorization request. Content: " + serverResponse.Content, new HubSpotError(serverResponse.StatusCode, serverResponse.Content), serverResponse.Content);

            if (serverResponse.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new HubSpotException("Error generating authentication token.", JsonConvert.DeserializeObject<HubSpotError>(serverResponse.Content), serverResponse.Content);

            //CEH
            // return serverResponse.Data;
            return new HubSpotToken(); // fudging this for now.
        }
    }
}