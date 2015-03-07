# ImageProxy
[![Build status](https://ci.appveyor.com/api/projects/status/8ew9bxvyvqaiakak?svg=true)](https://ci.appveyor.com/project/T-Alex/imageproxy)

Resizing and caching of images from web on the fly (WCF service)

## Structure of request
```
{service_url}/{image_size}/?url={image_url}
```
*image_size* - size in pixels: width x height or some of preset values (Icon, Small, Medium, Detail)

## Examples of usage
```
http://images.example.com/200x150/?url=https://www.google.com.ua/images/srpr/logo11w.png
http://images.example.com/Medium/?url=https://www.google.com.ua/images/srpr/logo11w.png
```

## Configuration (Web.config)
* **Icon** - size in pixels for Icon predefined value.
* **Small** - size in pixels for Small predefined value.
* **Medium** - size in pixels for Medium predefined value.
* **Detail** - size in pixels for Detail predefined value.
* **ImageCachePath** - path for cache of images.
* **ClientCacheMaxAge** - time span of client cache max age.

## License
ImageProxy is under the [MIT license](LICENSE.md).
