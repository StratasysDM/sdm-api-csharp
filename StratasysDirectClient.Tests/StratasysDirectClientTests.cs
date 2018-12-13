using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
		private StratasysDirectClient.Config _testConfig = new StratasysDirectClient.Config
		{
			ApiKey= "40c7086936345ef1ac786a82b99c4ab7", // Rapidity API Key for the Test environment goes here. Register at https://developers.stratasysdirect.com/
			BaseUrl= "https://test-api.stratasysdirect.com",
		};

		private StratasysDirectClient.Config _productionConfig = new StratasysDirectClient.Config
		{
			ApiKey = "ac387c6b10b220f75862061da82e9dcc",  // Rapidity API Key for the Production environment goes here. Register at https://developers.stratasysdirect.com/
			BaseUrl = "https://api.stratasysdirect.com",
		};

        public bool EnableProduction = false;

        private const string TEST_PART1 = "CAP.STL";
		private const string TEST_PART2 = "PANEL.STL";
		private const string TEST_PART3 = "CHASSIS.SLDPRT";
		private const string TEST_PART_ZIPPED = "ZIPPED.ZIP";

		public StratasysDirectClient.Config Config { get; set; }
		public string AppDataPath { get; set; }
		public string TestFilePath1 { get; set; }
		public string TestFilePath2 { get; set; }
		public string TestFilePath3 { get; set; }

		[TestInitialize]
		public void TestInitialize ()
		{
			Config = EnableProduction ? _productionConfig : _testConfig;

			AppDataPath = Path.Combine (GetAssemblyDirectory (), @"App_Data");
			TestFilePath1 = Path.Combine (AppDataPath, TEST_PART1);
			TestFilePath2 = Path.Combine (AppDataPath, TEST_PART2);
			TestFilePath3 = Path.Combine (AppDataPath, TEST_PART3);
		}

		[TestMethod]
		public void GetMaterials ()
		{
			var client = new StratasysDirectClient (Config);

			var getMaterialsResponse = client.GetMaterials ();
			Print.JSON (() => getMaterialsResponse);
		}

		[TestMethod]
		public void UploadFile_Without_Content ()
		{
			var client = new StratasysDirectClient (Config);

			var fileUploadResponse = client.UploadFiles (new string[] { });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_Without_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (Config);

			var fileUploadResponse = client.UploadFiles (new[] { TestFilePath1 });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_Without_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (Config);

			var fileUploadResponse = client.UploadFiles (new[] { TestFilePath1, TestFilePath2 });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_With_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (Config);
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
                            fileUnits = "Millimeters",
							notes = "Optimize orientation for \"shear force.\"",
							analyze = true,
							repair = false,
							quantity = 2,
						},
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_With_Native_CAD_File ()
		{
			var client = new StratasysDirectClient (Config);
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
							partStyleId = PartStyles.HDSL_Somos_NeXt_White,
						},
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_With_User_Credentials ()
		{
			var client = new StratasysDirectClient (Config);
			var clientSessionId = Guid.NewGuid ().ToString ();

			var accessToken = client.CreateAccessToken ("test-api@stratasysdirect.com", "Password12");
			Print.JSON (() => accessToken);

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
							partStyleId = PartStyles.HDSL_Somos_NeXt_White,
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
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
						}
					},
				});
			Print.JSON (() => fileUploadResponse);

			string landingPageUrl = fileUploadResponse.location + "&access_token=" + accessToken.access_token;
			Print.JSON (() => landingPageUrl);
		}

		[TestMethod]
		public void UploadFiles_With_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (Config);
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
							partStyleId = PartStyles.HDSL_Somos_NeXt_White,
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
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
						}
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_With_DuplicateParts ()
		{
			var client = new StratasysDirectClient (Config);
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
							partStyleId = PartStyles.HDSL_Somos_NeXt_White,
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
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
						}
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_For_PolyJet_MultiColor ()
		{
			var client = new StratasysDirectClient (Config);
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
							//partStyleId = "", // Default to Partner or Customer settings
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><note><to>John</to><from>Jane</from><heading>Notes</heading><body>Optimize for shear force</body></note>",
							quantity = 5,
							//partStyleId = "", // Default to Partner or Customer settings
						}
					},
				});

			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_With_Zipped_Attachment ()
		{
			var client = new StratasysDirectClient (Config);
			var clientSessionId = Guid.NewGuid ().ToString ();

			AppDataPath = Path.Combine (GetAssemblyDirectory (), @"App_Data");
			string zippedFilePath = Path.Combine (AppDataPath, TEST_PART_ZIPPED);

			var fileUploadResponse = client.UploadFiles (
				new[] { TestFilePath1, zippedFilePath },
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
							quantity = 1,
						},
						new FileUploadProperties ()
						{
							contentType = "Attachment",
							fileId = Guid.NewGuid ().ToString (),
							analyze = false,
						},
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void GetPricing_For_ClientSessionId ()
		{
			var client = new StratasysDirectClient (Config);
			var clientSessionId = Guid.NewGuid ().ToString ();

			var fileUploadResponse = client.UploadFiles (
				//new[] { TestFilePath1, TestFilePath2 },
				new[] { TestFilePath2 },
				new FileUploadRequest ()
				{
					contextId = Guid.NewGuid ().ToString (),
					clientSessionId = clientSessionId,
					files = new List<FileUploadProperties> ()
					{
						//new FileUploadProperties ()
						//{
						//	contentType = "Part",
						//	fileId = Guid.NewGuid ().ToString (),
						//	fileUnits = "Inches",
						//	notes = "Optimize orientation for shear force.",
						//	analyze = true,
						//	repair = false,
						//	quantity = 2,
						//	partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
						//},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for minimal supports.",
							analyze = true,
							repair = true,
							quantity = 3,
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
						}
                    },
				});
			Print.JSON (() => fileUploadResponse);

			var getPricingListResponse = client.GetPricingForClientSessionId (
				clientSessionId, 
				new List<string>
				{
					PartStyles.PolyJet_VeroBlue_Blue
				});
			Print.JSON (() => getPricingListResponse);
		}

		[TestMethod]
		public void GetPricing_For_Uploads ()
		{
			var client = new StratasysDirectClient (Config);
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
                            fileUnits = "Millimeters",
                            notes = "Optimize orientation for shear force.",
                            analyze = true,
                            repair = false,
                            quantity = 2,
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
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
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
						}
                    },
				});
			Print.JSON (() => fileUploadResponse);

			var createPricingListForUploads = new CreatePricingListForUploads ();
			createPricingListForUploads.uploads = new List<PricedUpload> ()
            {
                 new PricedUpload ()
                 {
					uploadId = fileUploadResponse.data.items[0].uploadId,
					quantity = 2,
					partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
                 },
                 new PricedUpload ()
                 {
					uploadId = fileUploadResponse.data.items[1].uploadId,
					quantity = 3,
					partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
               },
            };

			var getPricingListResponse = client.GetPricingForUploads (clientSessionId, createPricingListForUploads);
			Print.JSON (() => getPricingListResponse);
		}

		[TestMethod]
		public void GetPricing_For_Uploads_With_Invalid_Part ()
		{
			var client = new StratasysDirectClient (Config);
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
                            fileUnits = "", // Generates a PartMetricsError due to missing units
                            notes = "Optimize orientation for shear force.",
                            analyze = true,
                            repair = false,
                            quantity = 2,
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
                        },
                    },
				});
			Print.JSON (() => fileUploadResponse);

			var createPricingListForUploads = new CreatePricingListForUploads ();
			createPricingListForUploads.uploads = new List<PricedUpload> ()
            {
                 new PricedUpload ()
                 {
					uploadId = fileUploadResponse.data.items[0].uploadId,
					quantity = 2,
					partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
                 },
            };

			var getPricingListResponse = client.GetPricingForUploads (clientSessionId, createPricingListForUploads);
			Print.JSON (() => getPricingListResponse);

			Assert.AreEqual (getPricingListResponse.data.items[0].fileUnits, "none");
			Assert.AreEqual (getPricingListResponse.data.items[0].partStatus, "PartMetricsError");
			Assert.AreEqual (getPricingListResponse.data.items[0].pricedList["Rapid"].Count, 1);
		}

		[TestMethod]
		public void CreateRFQ_For_Uploads ()
		{
			var client = new StratasysDirectClient (Config);
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
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
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
							partStyleId = PartStyles.FDM_XD10_ABS_M30_Natural,
						},
						new FileUploadProperties ()
						{
							contentType = "Part",
							fileId = Guid.NewGuid ().ToString (),
							notes = "With native CAD file",
							analyze = true,
							repair = false,
							quantity = 2,
							partStyleId = PartStyles.HDSL_Somos_NeXt_White,
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

		[TestMethod]
		public void UploadFiles_With_InputErrors ()
		{
			var client = new StratasysDirectClient (Config);
			var clientSessionId = "291\"8aabf-7fb9-49be-b4ad-92966df0126b\r\n"; // NOTE: invalid " and \r\n characters

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
							filename = "te\"st", // NOTE: invalid " character
							fileId = Guid.NewGuid ().ToString (),
							fileUnits = "Inches",
							notes = "Optimize orientation for shear force.",
							analyze = true,
							repair = false,
							quantity = 2,
							partStyleId = PartStyles.HDSL_Somos_NeXt_White,
						},
					},
				});
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_Test ()
		{
			var client = new StratasysDirectClient (Config);
			var clientSessionId = Guid.NewGuid ().ToString ();

			var accessToken = client.CreateAccessToken ("test-api@stratasysdirect.com", "Password12");
			Print.JSON (() => accessToken);

			FileUploadResponse fileUploadResponse = new FileUploadResponse ();
			var filenames = new[] { TestFilePath1, TestFilePath2, TestFilePath3 };
            foreach (string filename in filenames)
            {
	            fileUploadResponse = client.UploadFiles (
		            new[] { filename },
		            new FileUploadRequest ()
					{
						clientSessionId = clientSessionId,
						files = new List<FileUploadProperties> ()
						{
							new FileUploadProperties ()
							{
								contentType = "Part", // Part, Attachment
								analyze = false,
							},
						},
					});
                Print.JSON(() => fileUploadResponse);
            }

			string landingPageUrl = fileUploadResponse.location + "&access_token=" + accessToken.access_token;
			Print.JSON (() => landingPageUrl);
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
