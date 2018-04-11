//
// Copyright (C) 2018 Toxnot PBC
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NON-INFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN // ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace com.toxnot
{
    /// <summary>
    /// Client for HPDC's API. Used to submit HPD data to the HPDC Builder service.
    ///
    /// To use, first send the user to the url given by GetAuthorizeQueryUrl(). This will allow the
    /// user to approve the app and the API will issue a code to the registered callback URL.
    /// 
    /// In the handler for the registered callback URL, pass the code to SetToken() and the HpdcClient object
    /// will fetch and set its access token, which is passed to all subsequent API calls.
    /// 
    /// At that point (i.e. with a valid access token), you can use an HpdcClient object to issue
    /// REST API calls to the HPDC service.
    /// </summary>
    public class HpdcClient
    {
        /// <summary>
        /// Instantiates a new HpdcClient object. The client will use the
        /// given apiEndpoint, clientId, and secret.
        /// </summary>
        /// <param name="apiEndpoint">The URL endpoint to use when accessing the service. e.g. https://dev.api.hpd-collaborative.org/</param>
        /// <param name="clientId">The HPDC-supplied client id used to authenticate the app accessing the APIs</param>
        /// <param name="secret">The HPDC-supplied secret used to authenticate the app accessing the APIs</param>
        public HpdcClient(string apiEndpoint, int clientId, string secret)
        {
            _apiEndpoint = apiEndpoint;
            _apiClientId = clientId;
            _apiSecret = secret;

            _accessToken = "";
        }

        // ===============================================================================

        public string AccessToken => _accessToken;

        // ===============================================================================

        /// <summary>
        /// When a user wants to authorize us to connect to their HPDC account,
        /// we need to redirect them to the URL returned by this method.
        /// 
        /// This will initiate a login sequence that will eventually redirect back to our
        /// "callback" URL, where we'll get a code to use to fetch an auth token.
        /// </summary>
        /// <returns>a URI</returns>
        public Uri GetAuthorizeQueryUrl()
        {
            return new Uri(
                _apiEndpoint +
                "/oauth/authorize?client_id=" +
                WebUtility.UrlEncode(_apiClientId.ToString()) +
                "&response_type=code&scope=" +
                WebUtility.UrlEncode("create-hpds edit-hpds view-hpds"));
        }

        // ===============================================================================

        /// <summary>
        /// Hits the HPDC oauth service and retrieves an access token to use
        /// when accessing HPDC APIs.
        /// 
        /// After this method succeeds, the object has a valid access token and
        /// can call API's.
        /// 
        /// This is called by our "callback" redirect URL, which the user is sent to
        /// when they log in to HPDC and grant us permission to access their account.
        /// </summary>
        /// <param name="code">The code as returned in the redirect request</param>
        public async Task SetToken(string code)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_apiEndpoint);

                var content = new FormUrlEncodedContent(new []
                {
                    new KeyValuePair<string, string>("grant_type",    "authorization_code"),
                    new KeyValuePair<string, string>("client_id",     _apiClientId.ToString()),
                    new KeyValuePair<string, string>("client_secret", _apiSecret),
                    new KeyValuePair<string, string>("code",          code),
                });

                var response = await httpClient.PostAsync("/oauth/token", content);

                var bodyString = await response.Content.ReadAsStringAsync();

                var parsedResponse = JsonObject.ParseJsonFragment(bodyString);

                _accessToken = parsedResponse["access_token"].ToString();
            }
        }

        // ===============================================================================

        // Implement HPDC REST Endpoints here

        // ===============================================================================

        /// <summary>
        /// The base URL to the HPDC API - the web server endpoint.
        /// </summary>
        private readonly string _apiEndpoint;

        /// <summary>
        /// The client id as assigned by HPDC
        /// </summary>
        private readonly int _apiClientId;

        /// <summary>
        /// The client secret as assigned by HPDC
        /// </summary>
        private readonly string _apiSecret;

        /// <summary>
        /// This is retrieved through an authorization workflow. We pass this token
        /// to each API we call to authorize the API call.
        /// </summary>
        private string _accessToken;
    }
}
