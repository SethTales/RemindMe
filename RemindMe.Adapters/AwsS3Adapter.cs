using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;

namespace RemindMe.Adapters
{
    public class AwsS3Adapter : IStorageAdapter
    {
        private readonly IAmazonS3 _s3Client;

        public AwsS3Adapter(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<string> GetObjectAsync(string bucketName, string key)
        {
            var getObjectResponse = await _s3Client.GetObjectAsync(bucketName, key);
            var reader = new StreamReader(getObjectResponse.ResponseStream);
            return reader.ReadToEnd();
        }
    }
}