using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace LendingClubDotNet.Client
{
	public static class RequestUtility
	{
		public static TResponse ExecuteGetRequest<TResponse>(string url, string authorizationToken)
		{
			HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create(url);

			webRequest.ContentType = "application/x-www-form-urlencoded";
			webRequest.Accept = "application/json";

			//webRequest.Headers.Add("Authorization", authorizationToken);

			string jsonResponse;

			using (WebResponse webResponse = webRequest.GetResponse())
			using (Stream str = webResponse.GetResponseStream())
			{
				if (str == null)
					throw new InvalidOperationException("ResponseStream was null.");

				using (StreamReader sr = new StreamReader(str))
					jsonResponse = sr.ReadToEnd();
			}

			return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
		}

		public static TResponse ExecutePostRequest<TRequest, TResponse>(TRequest requestValue, string url, string authorizationToken)
		{
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
			webRequest.Method = WebRequestMethods.Http.Post;

			webRequest.Accept = "application/json";
			webRequest.ContentType = "application/json";
			webRequest.Headers.Add("Authorization", authorizationToken);

			string jsonData = JsonConvert.SerializeObject(requestValue);
			using (Stream s = webRequest.GetRequestStream())
			{
				using (StreamWriter sw = new StreamWriter(s))
					sw.Write(jsonData);
			}

			string jsonResponse;
			using (WebResponse webResponse = webRequest.GetResponse())
			using (Stream str = webResponse.GetResponseStream())
			{
				if (str == null)
					throw new InvalidOperationException("ResponseStream was null.");

				using (StreamReader sr = new StreamReader(str))
					jsonResponse = sr.ReadToEnd();
			}

			return JsonConvert.DeserializeObject<TResponse>(jsonResponse);
		}
	}
}
