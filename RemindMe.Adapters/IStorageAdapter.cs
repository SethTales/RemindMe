using System.Threading.Tasks;
using Amazon.S3.Model;

namespace RemindMe.Adapters
{
    public interface IStorageAdapter
    {
        Task<string> GetObjectAsync(string bucketName, string key);
    }
}