using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Script.Serialization;

using Newtonsoft.Json;

using StratasysDirect.Models;

namespace StratasysDirect
{
	// TODO: gernerate api client proxy via https://github.com/Azure/autorest
	public class StratasysDirectClient
	{
		private readonly static TraceSource Log = new TraceSource ("StratasysDirect.StratasysDirectClient");

		private string acessToken = "/Rapidity/api/public/v1/oauth2/token";
		private string uploadUrl = "/Rapidity/api/public/v1/upload/files?uploadType=Multipart";
		private string materialsUrl = "/Rapidity/api/public/v1/products/simple";
		private string pricingCreateForClientSessionIdUrl = "/Rapidity/api/public/v1/pricing/commands/createForClientSessionId?clientSessionId=";
		private string pricingCreateForUploadsUrl = "/Rapidity/api/public/v1/pricing/commands/createForUploads?clientSessionId=";
		private string rfqCreateForClientSessionIdUrl = "/Rapidity/api/public/v1/rfq/commands/createForClientSessionId?clientSessionId=";
		private string rfqCreateForUploadsUrl = "/Rapidity/api/public/v1/rfq/commands/createForUploads?clientSessionId=";

		public StratasysDirectClient (string baseUrl, string apiKey)
		{
			BaseUrl = baseUrl;
			ApiKey = apiKey;
		}

		public string BaseUrl { get; set; }
		public string ApiKey { get; set; }

		public CreateAccessTokenResponse CreateAccessToken (string username, string password)
		{
			using (var client = CreateHttpClient ())
			{
				var parameters = new Dictionary<string, string> ()
				{
					{"grant_type", "password"},
					{"client_id", ApiKey},
					{"username", username},
					{"password", password},
				};

				using (var content = new FormUrlEncodedContent (parameters))
				{ 
					content.Headers.Clear ();
					content.Headers.ContentType = new MediaTypeHeaderValue ("application/x-www-form-urlencoded");

					var result = client.PostAsync (FormatUrl (acessToken), content).Result;
					Log.TraceInformation (result.ToString ());

					string response = result.Content.ReadAsStringAsync ().Result;
					Log.TraceInformation (response);

					var createAccessTokenResponse = new JavaScriptSerializer ().Deserialize<CreateAccessTokenResponse> (response);
					return createAccessTokenResponse;
				}
			}
		}

		public GetMaterialsResponse GetMaterials ()
		{
			using (var client = CreateHttpClient ())
			{
				var result = client.GetAsync (FormatUrl (materialsUrl)).Result;
				Log.TraceInformation (result.ToString ());

				string response = result.Content.ReadAsStringAsync ().Result;
				Log.TraceInformation (response);

				var getMaterialsResponse = new JavaScriptSerializer ().Deserialize<GetMaterialsResponse> (response);
				return getMaterialsResponse;
			}
		}

		public FileUploadResponse UploadFiles (string[] filenames, FileUploadRequest fileUploadRequest = null)
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

			using (var content = new MultipartFormDataContent ("Upload----" + DateTime.Now.ToString (CultureInfo.InvariantCulture)))
			{
				AddFiles (filenames, content);

				AddFormData (fileUploadRequest, content);

				using (var client = CreateHttpClient ())
				{
					var result = client.PostAsync (FormatUrl (uploadUrl), content).Result;
					Log.TraceInformation (result.ToString ());

					string response = result.Content.ReadAsStringAsync ().Result;
					Log.TraceInformation (response);

					// TODO: add ability to easily determine success/failure without checking errors == null
					var fileUploadResponse = new JavaScriptSerializer ().Deserialize<FileUploadResponse> (response);

					IEnumerable<string> values;
					if (result.Headers.TryGetValues ("Location", out values))
					{
						fileUploadResponse.location = values.FirstOrDefault ();
					}

					return fileUploadResponse;
				}
			}
		}

		private HttpClient CreateHttpClient ()
		{
			var client = new HttpClient ();
			client.DefaultRequestHeaders.Add ("X-SDM-ApiKey", ApiKey);
			client.DefaultRequestHeaders.Accept.Clear ();
			client.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
			return client;
		}

		private static void AddFiles (string[] filenames, MultipartFormDataContent content)
		{
			for (int index = 0; index < filenames.Length; index++)
			{
				string filename = filenames[index];
				var fileInfo = new FileInfo (filename);
				string name = string.Format ("files[{0}]", index);
				content.Add (new StreamContent (new MemoryStream (File.ReadAllBytes (filename))), name, fileInfo.Name);
			}
		}

		private void AddFormData (FileUploadRequest fileUploadRequest, MultipartFormDataContent content)
		{
			if (fileUploadRequest != null)
			{
				AddFormData (content, "contextId", fileUploadRequest.contextId);
				AddFormData (content, "clientSessionId", fileUploadRequest.clientSessionId);
				for (int index = 0; index < fileUploadRequest.files.Count; index++)
				{
					var file = fileUploadRequest.files[index];

					AddFormData (content, index, "contentType", file.contentType);
					AddFormData (content, index, "fileId", file.fileId);
					AddFormData (content, index, "fileUrl", file.fileUrl);
					AddFormData (content, index, "fileType", file.fileType);
					AddFormData (content, index, "filename", file.filename);
					AddFormData (content, index, "fileUnits", file.fileUnits);
					AddFormData (content, index, "displayUnits", file.displayUnits);
					AddFormData (content, index, "notes", file.notes);
					AddFormData (content, index, "analyze", file.analyze.ToString ());
					AddFormData (content, index, "repair", file.repair.ToString ());
					AddFormData (content, index, "quantity", file.quantity.ToString ());
					AddFormData (content, index, "materialTypeId", file.materialTypeId);
					AddFormData (content, index, "partStyleId", file.partStyleId);
				}
			}
		}

		public PricedPartListResponse GetPricingForClientSessionId (string clientSessionId)
		{
			var createPricingListForClientSessionId = new CreatePricingListForClientSessionId ()
			{
				clientSessionId = clientSessionId,
				partStylesIds = null, // Pass null to get pricing for all simple products
			};

			using (var client = CreateHttpClient ())
			{
				var result = client.PostAsync (FormatUrl (pricingCreateForClientSessionIdUrl) + clientSessionId, new JsonContent (createPricingListForClientSessionId)).Result;
				Log.TraceInformation (result.ToString ());

				string response = result.Content.ReadAsStringAsync ().Result;
				Log.TraceInformation (response);

				var getPricingListResponse = new JavaScriptSerializer ().Deserialize<PricedPartListResponse> (response);
				return getPricingListResponse;
			}
		}

		public PricedPartListResponse GetPricingForUploads (string clientSessionId, CreatePricingListForUploads createPricingListForUploads)
		{
			using (var client = CreateHttpClient ())
			{
				var result = client.PostAsync (FormatUrl (pricingCreateForUploadsUrl) + clientSessionId, new JsonContent (createPricingListForUploads)).Result;
				Log.TraceInformation (result.ToString ());

				string response = result.Content.ReadAsStringAsync ().Result;
				Log.TraceInformation (response);

				var getPricingListResponse = new JavaScriptSerializer ().Deserialize<PricedPartListResponse> (response);
				return getPricingListResponse;
			}
		}

		private void AddFormData (MultipartFormDataContent content, string name, string value)
		{
			if (string.IsNullOrEmpty (value) == false)
			{
				content.Add (new StringContent (value), string.Format ("\"{0}\"", name));
			}
		}

		private void AddFormData (MultipartFormDataContent content, int index, string name, string value)
		{
			if (string.IsNullOrEmpty (value) == false)
			{
				content.Add (new StringContent (value), string.Format ("\"files[{0}][{1}]\"", index, name));
			}
		}

		private string FormatUrl (string endpoint)
		{
			return string.Format ("{0}{1}", BaseUrl, endpoint);
		}
	}

	public class JsonContent : StringContent
	{
		public JsonContent (object obj)
			: base (JsonConvert.SerializeObject (obj), Encoding.UTF8, "application/json")
		{
		}
	}
}
