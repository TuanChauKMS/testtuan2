using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using QSR.NVivo.Plugins.PlatformsIdentity.Factory;
using QSR.NVivo.Plugins.PlatformsIdentity.Responses.CloudAccounts;
using QSR.NVivo.Plugins.PlatformsIdentity.Services;
using RestSharp;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Test.Services
{
	[TestClass]
	public class CloudAccountsServiceProxyTest
	{
		[TestMethod]
		[DeploymentItem(@"..\..\TestData\myprofile.json")]
		public void GetAccountId_ValidRequestParams_Success()
		{
			/*---ARRANGE---*/

			var restFactoryMock = new Mock<IRestClientFactory>();
			var restClientMock = new Mock<IRestClient>();
			restFactoryMock.Setup(x => x.Create(It.IsAny<object>())).Returns(restClientMock.Object);

			ICloudAccountsServiceProxy serviceProxy = new CloudAccountsServiceProxy(restFactoryMock.Object);
			string responseContent;
			using (var reader = new StreamReader("myprofile.json"))
			{
				responseContent = reader.ReadToEnd();
			}

			IRestResponse response = new RestResponse
			{
				StatusCode = HttpStatusCode.OK,
				Content = responseContent
			};

			var expectedResponse = JsonConvert.DeserializeObject<MyProfileResponse>(responseContent);

			IRestRequest restRequest = null;

			restClientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
				.Callback<IRestRequest>(request => restRequest = request)
				.Returns(response);

			/*---ACT---*/

			var accountId = serviceProxy.GetAccountId("accesstoken");

			/*---ASSERT---*/

			var parameterList = restRequest?.Parameters;
			Assert.AreEqual(expectedResponse.UserProfile.AccountId, accountId);
			Assert.AreEqual(3, parameterList.Count);
			Assert.IsTrue(parameterList.Any(x => x.Type == ParameterType.HttpHeader && x.Name == "Authorization"));
			Assert.IsTrue(parameterList.Any(x => x.Type == ParameterType.HttpHeader && x.Name == "Ocp-Apim-Subscription-Key"));
			Assert.IsTrue(parameterList.Any(x => x.Type == ParameterType.HttpHeader && x.Name == "Ocp-Apim-Trace"));
		}

		[TestMethod]
		[DeploymentItem(@"..\..\TestData\RemainingCreditResponse.json")]
		public void GetRemainingCreditTest_ValidRequestParams_Success()
		{
			/*---ARRANGE---*/

			var restFactoryMock = new Mock<IRestClientFactory>();
			var restClientMock = new Mock<IRestClient>();
			restFactoryMock.Setup(x => x.Create(It.IsAny<object>())).Returns(restClientMock.Object);

			ICloudAccountsServiceProxy serviceProxy = new CloudAccountsServiceProxy(restFactoryMock.Object);
			string responseContent;
			using (var reader = new StreamReader("RemainingCreditResponse.json"))
			{
				responseContent = reader.ReadToEnd();
			}

			IRestResponse response = new RestResponse
			{
				StatusCode = HttpStatusCode.OK,
				Content = responseContent
			};

			var expectedResponse = JsonConvert.DeserializeObject<RemainingCreditResponse>(responseContent);

			IRestRequest restRequest = null;

			restClientMock.Setup(x => x.Execute(It.IsAny<IRestRequest>()))
				.Callback<IRestRequest>(request => restRequest = request)
				.Returns(response);

			const int productId = 1;

			/*---ACT---*/

			var remainingCredit = serviceProxy.GetRemainingCredit("accesstoken", 657, productId);

			/*---ASSERT---*/

			var parameterList = restRequest?.Parameters;
			Assert.AreEqual(expectedResponse.RemainingCreditsByProduct.First(x => x.ProductId == productId).RemainingCredits, remainingCredit);
			Assert.AreEqual(4, parameterList.Count);
			Assert.IsTrue(parameterList.Any(x => x.Type == ParameterType.HttpHeader && x.Name == "Authorization"));
			Assert.IsTrue(parameterList.Any(x => x.Type == ParameterType.HttpHeader && x.Name == "Ocp-Apim-Subscription-Key"));
			Assert.IsTrue(parameterList.Any(x => x.Type == ParameterType.HttpHeader && x.Name == "Ocp-Apim-Trace"));
			Assert.IsTrue(parameterList.Any(x => x.Type == ParameterType.GetOrPost && x.Name == "productId" && (int)x.Value == 1));
		}
	}
}
