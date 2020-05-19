
using System.IO;
using System.Threading.Tasks;

namespace TAlex.ImageResizer.Service
{
    public interface IImageResizerService
    {
        Task<Stream> ResizeAsync(string url, string size);
    }
}
