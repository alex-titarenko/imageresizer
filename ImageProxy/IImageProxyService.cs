using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;


namespace TAlex.ImageProxy
{
    [ServiceContract]
    public interface IImageProxyService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/{size}/?url={url}", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetImage(string size, string url);
    }
}
