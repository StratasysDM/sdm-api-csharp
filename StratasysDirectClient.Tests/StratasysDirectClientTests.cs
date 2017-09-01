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
		private const string TEST_PART3 = "CHASSIS.SLDPRT";

		public string ApiKey = "";
		public string BaseUrl = "";
		public string AppDataPath { get; set; }
		public string TestFilePath1 { get; set; }
		public string TestFilePath2 { get; set; }
		public string TestFilePath3 { get; set; }

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
			TestFilePath3 = Path.Combine (AppDataPath, TEST_PART3);
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
			var clientSessionId = Guid.NewGuid ().ToString ();
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
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
							materialTypeId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							partStyleId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_With_Native_CAD_File ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var clientSessionId = Guid.NewGuid ().ToString ();
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath3 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
					files = new List<FileUploadProperties> ()
					{
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							notes = "With native CAD file",
							analyze = true,
							repair = false,
							quantity = 2,
							materialTypeId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							partStyleId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_With_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var clientSessionId = Guid.NewGuid ().ToString ();
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1, TestFilePath2 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
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
							materialTypeId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							partStyleId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for minimal supports.",
							analyze = true,
							repair = true,
							quantity = 3,
							materialTypeId = "18e1b04b-49da-4300-b93f-fadd812d38a9",
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
						}
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_With_DuplicateParts ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var clientSessionId = Guid.NewGuid ().ToString ();
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1, TestFilePath1 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
					files = new List<FileUploadProperties> ()
					{
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "",
							analyze = true,
							repair = false,
							quantity = 2,
							materialTypeId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							partStyleId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "",
							analyze = true,
							repair = false,
							quantity = 3,
							materialTypeId = "18e1b04b-49da-4300-b93f-fadd812d38a9",
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
						}
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_For_PolyJet_MultiColor ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var clientSessionId = Guid.NewGuid ().ToString ();
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1, TestFilePath2 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
					files = new List<FileUploadProperties> ()
					{
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for shear force.",
							quantity = 1,
							materialTypeId = "901393d6-ed84-4031-8785-4239ab9fa281",
							partStyleId = "045507ae-1149-47a6-9a02-7ad9e04f21c3",
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><note><to>John</to><from>Jane</from><heading>Notes</heading><body>Optimize for shear force</body></note>",
							quantity = 5,
							materialTypeId = "901393d6-ed84-4031-8785-4239ab9fa281",
							partStyleId = "045507ae-1149-47a6-9a02-7ad9e04f21c3",
						}
					},
				});

			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void GetPricingForClientSessionId ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var clientSessionId = Guid.NewGuid ().ToString ();
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1, TestFilePath2 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
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
							materialTypeId = "18e1b04b-49da-4300-b93f-fadd812d38a9",
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
                        },
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for minimal supports.",
							analyze = true,
							repair = true,
							quantity = 3,
							materialTypeId = "18e1b04b-49da-4300-b93f-fadd812d38a9",
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
						}
                    },
				});
			Print.JSON (() => fileUploadResponse);

			var getPricingListResponse = client.GetPricingForClientSessionId (clientSessionId);
			Print.JSON (() => getPricingListResponse);
		}

        [TestMethod]
        public void GetPricingForUploads ()
        {
            var client = new StratasysDirectClient(BaseUrl, ApiKey);
            var clientSessionId = Guid.NewGuid().ToString();
            var fileUploadResponse = client.UploadFiles(
                new[] { TestFilePath1, TestFilePath2 },
                new FileUploadRequest()
                {
                    contextId = Guid.NewGuid().ToString(),
                    clientSessionId = clientSessionId,
                    files = new List<FileUploadProperties>()
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
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
                        },
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for minimal supports.",
							analyze = true,
							repair = true,
							quantity = 3,
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
						}
                    },
                });
            Print.JSON(() => fileUploadResponse);

            var createPricingListForUploads = new CreatePricingListForUploads();
            createPricingListForUploads.uploads = new List<PricedUpload>()
            {
                 new PricedUpload ()
                 {
                   uploadId = fileUploadResponse.data.items[0].uploadId,
                   quantity = 2,
                   partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e"
                 },
                 new PricedUpload ()
                 {
                   uploadId = fileUploadResponse.data.items[1].uploadId,
                   quantity = 3,
                   partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
                 },
            };

            var getPricingListResponse = client.GetPricingForUploads (clientSessionId, createPricingListForUploads);

            Print.JSON(() => getPricingListResponse);
        }

        [TestMethod]
        public void GetPricingForUploads_With_Invalid_Part ()
        {
            var client = new StratasysDirectClient(BaseUrl, ApiKey);
            var clientSessionId = Guid.NewGuid().ToString();
            var fileUploadResponse = client.UploadFiles(
                new[] { TestFilePath1 },
                new FileUploadRequest()
                {
                    contextId = Guid.NewGuid().ToString(),
                    clientSessionId = clientSessionId,
                    files = new List<FileUploadProperties>()
                    {
                        new FileUploadProperties ()
                        {
                            contentType = "Part",
                            fileId = Guid.NewGuid ().ToString (),
                            fileUnits = "", // Generates a PartMetricsError due to missing units
                            notes = "Optimize orientation for shear force.",
                            analyze = true,
                            repair = false,
                            quantity = 2,
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
                        },
                    },
                });
            Print.JSON(() => fileUploadResponse);

            var createPricingListForUploads = new CreatePricingListForUploads();
            createPricingListForUploads.uploads = new List<PricedUpload>()
            {
                 new PricedUpload ()
                 {
                   uploadId = fileUploadResponse.data.items[0].uploadId,
                   quantity = 2,
                   partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e"
                 },
            };

            var getPricingListResponse = client.GetPricingForUploads (clientSessionId, createPricingListForUploads);

            Print.JSON(() => getPricingListResponse);

			Assert.AreEqual (getPricingListResponse.data.items[0].fileUnits, "none");
			Assert.AreEqual (getPricingListResponse.data.items[0].partStatus, "PartMetricsError");
			Assert.AreEqual (getPricingListResponse.data.items[0].pricedList["Rapid"].Count, 0);
        }

        [TestMethod]
		public void CreateRFQForUploads ()
		{
			var client = new StratasysDirectClient (BaseUrl, ApiKey);
			var clientSessionId = Guid.NewGuid ().ToString ();
			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1, TestFilePath2, TestFilePath3 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
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
							materialTypeId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							partStyleId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "",
							analyze = true,
							repair = false,
							quantity = 3,
							materialTypeId = "18e1b04b-49da-4300-b93f-fadd812d38a9",
							partStyleId = "1a5f7a14-a08d-4641-a6a6-641fe129fb9e",
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							notes = "With native CAD file",
							analyze = true,
							repair = false,
							quantity = 2,
							materialTypeId = "d2b39205-332e-46ae-bc67-eae7ccc54b89",
							partStyleId = "8a314c28-2d7e-4ecd-9114-2125b6d28709",
						},
					},
				});
			Print.JSON (() => fileUploadResponse);

			var createPricingListForUploads = new CreatePricingListForUploads ();
			fileUploadResponse.data.items.ForEach (upload =>
			{
				createPricingListForUploads.uploads.Add (new PricedUpload ()
				{
					uploadId = upload.uploadId,
				});
			});

			var getPricingListResponse = client.GetPricingForUploads (clientSessionId, createPricingListForUploads);
			Print.JSON (() => getPricingListResponse);
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
