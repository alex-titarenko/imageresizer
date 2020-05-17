
using System.IO;
using System.Threading.Tasks;

namespace TAlex.ImageProxy
{
    public interface IImageProxyService
    {
        Task<Stream> GetImageAsync(string size, string url);
    }
}
