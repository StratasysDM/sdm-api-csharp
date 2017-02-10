using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Script.Serialization;

using StratasysDirect.Models;

namespace StratasysDirect
{
	// TODO: gernerate api client proxy via https://github.com/Azure/autorest
	public class StratasysDirectClient
	{
		private string uploadUrl = "/Rapidity/api/public/v1/upload/files?uploadType=Multipart";
		private string materialsUrl = "/Rapidity/api/public/v1/products/express";

		public StratasysDirectClient (string baseUrl, string apiKey)
		{
			BaseUrl = baseUrl;
			ApiKey = apiKey;
		}

		public string BaseUrl { get; set; }
		public string ApiKey { get; set; }

		public GetMaterialsResponse GetMaterials ()
		{
			using (var client = CreateHttpClient ())
			{
				var result = client.GetAsync (FormatUrl (materialsUrl)).Result;

				string response = result.Content.ReadAsStringAsync ().Result;
				//Trace.WriteLine (response);

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
					//Trace.WriteLine (result);

					IEnumerable<string> values;
					string location = string.Empty;
					if (result.Headers.TryGetValues ("Location", out values))
					{
						location = values.FirstOrDefault ();
						//Trace.WriteLine (string.Format ("Location: {0}", location));
					}

					string response = result.Content.ReadAsStringAsync ().Result;
					//Trace.WriteLine (response);

					// TODO: add ability to easily determine success/failure without checking errors == null
					var fileUploadResponse = new JavaScriptSerializer ().Deserialize<FileUploadResponse> (response);
					return fileUploadResponse;
				}
			}
		}

		private HttpClient CreateHttpClient ()
		{
			var client = new HttpClient ();
			client.DefaultRequestHeaders.Add ("X-SDM-ApiKey", ApiKey);
			return client;
		}

		private static void AddFiles (string[] filenames, MultipartFormDataContent content)
		{
			for (int index = 0; index < filenames.Length; index++)
			{
				string filename = filenames[index];
				var fileInfo = new FileInfo (filename);
				string name = string.Format ("xyz[{0}]", index);
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
					AddFormData (content, index, "materialId", file.materialId);
					AddFormData (content, index, "finishId", file.finishId);
				}
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
}
