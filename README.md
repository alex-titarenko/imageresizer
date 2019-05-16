# ImageProxy
[![Build status](https://ci.appveyor.com/api/projects/status/8ew9bxvyvqaiakak?svg=true)](https://ci.appveyor.com/project/alex-titarenko/imageproxy)

Resizing and caching images from web on the fly (WCF service).

## Structure of request
```
{service_url}/{image_size}/?url={image_url}
```
*image_size* - size in pixels: `width x height` or some of preset values (Small, Medium, Detail, Original)

*image_url* - an URL for image to resizing (can be encoded in base64 format).

## Examples of usage
```
http://images.example.com/200x150/?url=https://www.google.com.ua/images/srpr/logo11w.png
http://images.example.com/Medium/?url=https://www.google.com.ua/images/srpr/logo11w.png
http://images.example.com/Original/?url=https://www.google.com.ua/images/srpr/logo11w.png
http://images.example.com/Original/?url=base64:aHR0cHM6Ly93d3cuZ29vZ2xlLmNvbS51YS9pbWFnZXMvc3Jwci9sb2dvMTF3LnBuZw==
```

## Configuration (Web.config)
* **Small** - size in pixels for Small predefined value.
* **Medium** - size in pixels for Medium predefined value.
* **Detail** - size in pixels for Detail predefined value.
* **UseLocalCache** - a boolean flag that indicates whether to use local file cache.
* **LocalCachePath** - folder path to local file cache.
* **ClientCacheMaxAge** - time span of client cache max age.
* **UserAgent** - user agent string for downloading images.

## License
ImageProxy is under the [MIT license](LICENSE.md).
