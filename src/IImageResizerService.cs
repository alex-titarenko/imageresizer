
using System.IO;
using System.Threading.Tasks;

namespace TAlex.ImageProxy
{
    public interface IImageResizerService
    {
        Task<Stream> ResizeAsync(string size, string url);
    }
}
