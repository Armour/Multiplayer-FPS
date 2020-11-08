// ----------------------------------------------------------------------------
// <copyright file="AccountService.cs" company="Exit Games GmbH">
//   Photon Cloud Account Service - Copyright (C) 2012 Exit Games GmbH
// </copyright>
// <summary>
//   Provides methods to register a new user-account for the Photon Cloud and
//   get the resulting appId.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Photon.Pun
{
    /// <summary>
    /// Creates a instance of the Account Service to register Photon Cloud accounts.
    /// </summary>
    public class AccountService
    {
        private const string ServiceUrl = "https://partner.photonengine.com/api/{0}/User/RegisterEx";

        private readonly Dictionary<string, string> RequestHeaders = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
            { "x-functions-key", "" }
        };

        private const string DefaultContext = "Unity";

        private const string DefaultToken = "VQ920wVUieLHT9c3v1ZCbytaLXpXbktUztKb3iYLCdiRKjUagcl6eg==";

        /// <summary>
        /// third parties custom context, if null, defaults to DefaultContext property value
        /// </summary>
        public string CustomContext = null;
        
        /// <summary>
        /// third parties custom token. If null, defaults to DefaultToken property value
        /// </summary>
        public string CustomToken = null;
        
        
        /// <summary>
        /// Attempts to create a Photon Cloud Account asynchronously.
        /// Once your callback is called, check ReturnCode, Message and AppId to get the result of this attempt.
        /// </summary>
        /// <param name="email">Email of the account.</param>
        /// <param name="serviceTypes">Defines which type of Photon-service is being requested.</param>
        /// <param name="callback">Called when the result is available.</param>
        public bool RegisterByEmail(string email, string serviceTypes, Action<AccountServiceResponse> callback = null, Action<string> errorCallback = null)
        {
            if (!IsValidEmail(email))
            {
                Debug.LogErrorFormat("Email \"{0}\" is not valid", email);
                return false;
            }
            if (string.IsNullOrEmpty(serviceTypes))
            {
                Debug.LogError("serviceTypes string is null or empty");
                return false;
            }
            AccountServiceRequest req = new AccountServiceRequest();
            req.Email = email;
            req.ServiceTypes = serviceTypes;
            //Debug.LogWarningFormat("Service types sent {0}", serviceTypes);
            return this.RegisterByEmail(req, callback, errorCallback);
        }

        public bool RegisterByEmail(string email, List<ServiceTypes> serviceTypes, Action<AccountServiceResponse> callback = null, Action<string> errorCallback = null)
        {
            if (serviceTypes == null || serviceTypes.Count == 0)
            {
                Debug.LogError("serviceTypes list is null or empty");
                return false;
            }
            return this.RegisterByEmail(email, GetServiceTypesFromList(serviceTypes), callback, errorCallback);
        }

        public bool RegisterByEmail(AccountServiceRequest request, Action<AccountServiceResponse> callback = null, Action<string> errorCallback = null)
        {
            if (request == null)
            {
                Debug.LogError("Registration request is null");
                return false;
            }
            string fullUrl = GetUrlWithQueryStringEscaped(request);

            RequestHeaders["x-functions-key"] = string.IsNullOrEmpty(CustomToken) ? DefaultToken : CustomToken;
            
            //Debug.LogWarningFormat("Full URL {0}", fullUrl);
            PhotonEditorUtils.StartCoroutine(
                PhotonEditorUtils.HttpPost(fullUrl,
                    RequestHeaders,
                    null,
                    s =>
                    {
                        //Debug.LogWarningFormat("received response {0}", s);
                        if (string.IsNullOrEmpty(s))
                        {
                            if (errorCallback != null)
                            {
                                errorCallback("Server's response was empty. Please register through account website during this service interruption.");
                            }
                        }
                        else
                        {
                            AccountServiceResponse ase = this.ParseResult(s);
                            if (ase == null)
                            {
                                if (errorCallback != null)
                                {
                                    errorCallback("Error parsing registration response. Please try registering from account website");
                                }
                            }
                            else if (callback != null)
                            {
                                callback(ase);
                            }
                        }
                    },
                    e =>
                    {
                        if (errorCallback != null)
                        {
                            errorCallback(e);
                        }
                    })
            );
            return true;
        }

        private string GetUrlWithQueryStringEscaped(AccountServiceRequest request)
        {
            string email = UnityEngine.Networking.UnityWebRequest.EscapeURL(request.Email);
            string st = UnityEngine.Networking.UnityWebRequest.EscapeURL(request.ServiceTypes);
            string serviceUrl = string.Format(ServiceUrl, string.IsNullOrEmpty(CustomContext) ? DefaultContext : CustomContext );
            return string.Format("{0}?email={1}&st={2}", serviceUrl, email, st);
        }

        /// <summary>
        /// Reads the Json response and applies it to local properties.
        /// </summary>
        /// <param name="result"></param>
        private AccountServiceResponse ParseResult(string result)
        {
            try
            {
                AccountServiceResponse res = JsonUtility.FromJson<AccountServiceResponse>(result);
                // Unity's JsonUtility does not support deserializing Dictionary, we manually parse it, dirty & ugly af, better then using a 3rd party lib
                if (res.ReturnCode == AccountServiceReturnCodes.Success)
                {
                    string[] parts = result.Split(new[] { "\"ApplicationIds\":{" }, StringSplitOptions.RemoveEmptyEntries);
                    parts = parts[1].Split('}');
                    string applicationIds = parts[0];
                    if (!string.IsNullOrEmpty(applicationIds))
                    {
                        parts = applicationIds.Split(new[] { ',', '"', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        res.ApplicationIds = new Dictionary<string, string>(parts.Length / 2);
                        for (int i = 0; i < parts.Length; i = i + 2)
                        {
                            res.ApplicationIds.Add(parts[i], parts[i + 1]);
                        }
                    }
                    else
                    {
                        Debug.LogError("The server did not return any AppId, ApplicationIds was empty in the response.");
                        return null;
                    }
                }
                return res;
            }
            catch (Exception ex) // probably JSON parsing exception, check if returned string is valid JSON
            {
                Debug.LogException(ex);
                return null;
            }
        }

        private static string GetServiceTypesFromList(List<ServiceTypes> appTypes)
        {
            if (appTypes != null)
            {
                string serviceTypes = string.Empty;
                if (appTypes.Count > 0)
                {
                    serviceTypes = ((int)appTypes[0]).ToString();
                    for (int i = 1; i < appTypes.Count; i++)
                    {
                        int appType = (int)appTypes[i];
                        serviceTypes = string.Format("{0},{1}", serviceTypes, appType);
                    }
                }
                return serviceTypes;
            }
            return null;
        }

        // RFC2822 compliant matching 99.9% of all email addresses in actual use today
        // according to http://www.regular-expressions.info/email.html [22.02.2012]
        private static Regex reg = new Regex("^((?>[a-zA-Z\\d!#$%&'*+\\-/=?^_{|}~]+\\x20*|\"((?=[\\x01-\\x7f])[^\"\\]|\\[\\x01-\\x7f])*\"\\x20*)*(?<angle><))?((?!\\.)(?>\\.?[a-zA-Z\\d!#$%&'*+\\-/=?^_{|}~]+)+|\"((?=[\\x01-\\x7f])[^\"\\]|\\[\\x01-\\x7f])*\")@(((?!-)[a-zA-Z\\d\\-]+(?<!-)\\.)+[a-zA-Z]{2,}|\\[(((?(?<!\\[)\\.)(25[0-5]|2[0-4]\\d|[01]?\\d?\\d)){4}|[a-zA-Z\\d\\-]*[a-zA-Z\\d]:((?=[\\x01-\\x7f])[^\\\\[\\]]|\\[\\x01-\\x7f])+)\\])(?(angle)>)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        public static bool IsValidEmail(string mailAddress)
        {
            if (string.IsNullOrEmpty(mailAddress))
            {
                return false;
            }
            var result = reg.Match(mailAddress);
            return result.Success;
        }
    }

    [Serializable]
    public class AccountServiceResponse
    {
        public int ReturnCode;
        public string Message;
        public Dictionary<string, string> ApplicationIds; // Unity's JsonUtility does not support deserializing Dictionary
    }
    
    [Serializable]
    public class AccountServiceRequest
    {
        public string Email;
        public string ServiceTypes;
    }

    public class AccountServiceReturnCodes
    {
        public static int Success = 0;
        public static int EmailAlreadyRegistered = 8;
        public static int InvalidParameters = 12;
    }

    public enum ServiceTypes
    {
        Realtime = 0,
        Turnbased = 1,
        Chat = 2,
        Voice = 3,
        TrueSync = 4,
        Pun = 5,
        Thunder = 6,
        Bolt = 20
    }
}

#endif