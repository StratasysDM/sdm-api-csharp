using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StratasysDirect;
using StratasysDirect.Extensions;
using StratasysDirect.Models;

namespace StratasysDirectClientTests
{
	[TestClass]
	public class StratasysDirectClientTests
	{
		private const string API_KEY_TEST = ""; // Rapidity API Key for the Test environment goes here
		private const string BASE_URL_TEST = "https://test-api.stratasysdirect.com";
		private const string API_KEY_PRODUCTION = ""; //  // Rapidity API Key for the Production environment goes here
		private const string BASE_URL_PRODUCTION = "https://api.stratasysdirect.com";

		private const string TEST_PART1 = "CAP.STL";
		private const string TEST_PART2 = "PANEL.STL";

		public string ApiKey = "";
		public string BaseUrl = "";
		public string AppDataPath { get; set; }
		public string TestFilePath1 { get; set; }
		public string TestFilePath2 { get; set; }

		[TestInitialize]
		public void TestInitialize ()
		{
            ApiKey = API_KEY_TEST;
            BaseUrl = BASE_URL_TEST;
            //ApiKey = API_KEY_PRODUCTION;
            //BaseUrl = BASE_URL_PRODUCTION;

            AppDataPath = Path.Combine (GetAssemblyDirectory (), @"App_Data");
			TestFilePath1 = Path.Combine (AppDataPath, TEST_PART1);
			TestFilePath2 = Path.Combine (AppDataPath, TEST_PART2);
		}

		[TestMethod]
		public void GetMaterials ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var getMaterialsResponse = client.GetMaterials ();
			Print.JSON (() => getMaterialsResponse);
		}

		[TestMethod]
		public void UploadFile_Without_Content ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var fileUploadResponse = client.UploadFiles (new string[] { });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_Without_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var fileUploadResponse = client.UploadFiles (new[] { TestFilePath1 });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_Without_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var fileUploadResponse = client.UploadFiles (new[] { TestFilePath1, TestFilePath2 });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_With_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					files = new List<FileUploadProperties> ()
					{
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for \"shear force.\"",
							analyze = true,
							repair = false,
							quantity = 2,
							materialId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							finishId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_With_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1, TestFilePath2 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					files = new List<FileUploadProperties> ()
					{
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for shear force.",
							analyze = true,
							repair = false,
							quantity = 2,
							materialId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							finishId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for minimal supports.",
							analyze = false,
							repair = true,
							quantity = 3,
							materialId = "18e1b04b-49da-4300-b93f-fadd812d38a9",
							finishId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
						}
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		internal static string GetAssemblyDirectory ()
		{
			string codeBase = Assembly.GetExecutingAssembly ().CodeBase;
			UriBuilder uri = new UriBuilder (codeBase);
			string path = Uri.UnescapeDataString (uri.Path);
			return Path.GetDirectoryName (path);
		}
	}

	public static class Print
	{
		public static void JSON (Expression<Func<object>> memberExpression)
		{
			MemberExpression expressonBody = memberExpression.Body as MemberExpression;

			var objectMember = Expression.Convert (expressonBody, typeof (object));
			var getterLambda = Expression.Lambda<Func<object>> (objectMember);
			var getter = getterLambda.Compile ();
			var value = getter ();

			Debug.WriteLine ("{0} = {1}", expressonBody.Member.Name, value.ToJSON ());
		}
	}
}
