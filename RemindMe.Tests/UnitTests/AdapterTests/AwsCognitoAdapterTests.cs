using NUnit.Framework;
using NSubstitute;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using RemindMe.Adapters.Helpers;
using RemindMe.Adapters;
using System.Threading.Tasks;
using RemindMe.Models;
using System.Collections.Generic;

namespace RemindMe.Tests.UnitTests.Adapters
{
    [TestFixture]
    public class AwsCognitoAdapterTests
    {
        private IAmazonCognitoIdentityProvider _awsCognitoClient;
        private AwsCognitoAdapterConfig _cognitoAdapterConfig;
        private IAwsCognitoAdapterHelper _cognitoAdapterHelper;
        private IAuthAdapter _authAdapter;

        [SetUp]
        public void Setup()
        {
            _cognitoAdapterConfig = new AwsCognitoAdapterConfig
            {
                UserPoolId = "fake-userpool-id",
                ClientId = "fake-client-id"
            };
            _awsCognitoClient = Substitute.For<IAmazonCognitoIdentityProvider>();
        }
    }
}