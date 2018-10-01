﻿using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TelimenaClient.Serializer;

namespace TelimenaClient
{
    internal class Messenger : IMessenger
    {
        public Messenger(ITelimenaSerializer serializer, ITelimenaHttpClient httpClient)
        {
            this.Serializer = serializer;
            this.HttpClient = httpClient;
        }

        public ITelimenaSerializer Serializer { get; }
        public ITelimenaHttpClient HttpClient { get; }

        public async Task<string> SendPostRequest(string requestUri, object objectToPost)
        {
            try
            {
                string jsonObject = this.Serializer.Serialize(objectToPost);
                StringContent content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.HttpClient.PostAsync(requestUri, content).ConfigureAwait(false);
                string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return responseContent;
            }
            catch (Exception ex)
            {
               throw new InvalidOperationException($"An error occured while posting to [{requestUri}]",ex);
            }
        }

        public async Task<string> SendGetRequest(string requestUri)
        {
            try
            {
                HttpResponseMessage response = await this.HttpClient.GetAsync(requestUri).ConfigureAwait(false);
                string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return responseContent;
            }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"An error occured while getting from [{requestUri}]", ex);
                }
        }

        public async Task<Stream> DownloadFile(string requestUri)
        {
            try
            {
                HttpResponseMessage response = await this.HttpClient.GetAsync(requestUri).ConfigureAwait(false);
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occured while downloading from [{requestUri}]", ex);
            }
        }
    }
}