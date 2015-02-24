# ImageProxy
Resize and caching image from web on the fly (WCF service)

## Structure of request
```
{service_url}/{size}/?url={image_url}
```
*size* - size in pixels: width x height or some of preset values (Icon, Small, Medium, Detail)

## Examples of usage
```
http://images.example.com/200x150/?url=https://www.google.com.ua/images/srpr/logo11w.png
http://images.example.com/Medium/?url=https://www.google.com.ua/images/srpr/logo11w.png
```
