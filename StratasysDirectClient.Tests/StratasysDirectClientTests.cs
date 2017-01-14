using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using StratasysDirect;
using StratasysDirect.Extensions;
using StratasysDirect.Models;

namespace StratasysDirectClientTests
{
	[TestClass]
	public class StratasysDirectClientTests
	{
		private const string _apiKey = ""; // SDM API Key goes here

		[TestMethod]
		public void GetMaterials ()
		{
			var client = new StratasysDirectClient (_apiKey);
			var getMaterialsResponse = client.GetMaterials ();
			Print.JSON (() => getMaterialsResponse);
		}

		[TestMethod]
		public void UploadFile_Without_Content ()
		{
			var client = new StratasysDirectClient (_apiKey);
			var fileUploadResponse = client.UploadFiles (new string[] { });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_Without_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (_apiKey);
			var fileUploadResponse = client.UploadFiles (new[] { @"E:\Parts\CAP.STL" });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFiles_Without_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (_apiKey);
			var fileUploadResponse = client.UploadFiles (new[] { @"E:\Parts\CAP.STL", @"E:\Parts\PANEL.STL" });
			Print.JSON (() => fileUploadResponse);
		}

		[TestMethod]
		public void UploadFile_With_FileUploadProperties ()
		{
			var client = new StratasysDirectClient (_apiKey);
			var fileUploadResponse = client.UploadFiles (
				new[] { @"E:\Parts\CAP.STL" },
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
			var client = new StratasysDirectClient (_apiKey);
			var fileUploadResponse = client.UploadFiles (
				new[] { @"E:\Parts\CAP.STL", @"E:\Parts\PANEL.STL" },
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
							fileUnits = "Millimeters",
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
