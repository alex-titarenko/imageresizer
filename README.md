# ImageResizer Service

Resize and cache images from the web on the fly (Azure Function V3).

## Structure of request
```
{service_url}/api/resizeimage?url={image_url}&size={image_size}
```
*image_url* - an URL for image to resizing (can be encoded in base64 format).

*image_size* - size in pixels: `width x height` or some of preset values (Small, Medium, Detail, Original)

## Examples of usage
```
http://images.example.com?url=https://www.google.com.ua/images/srpr/logo11w.png&size=200x150
http://images.example.com?url=https://www.google.com.ua/images/srpr/logo11w.png&size=medium
http://images.example.com?url=https://www.google.com.ua/images/srpr/logo11w.png&size=original
http://images.example.com?url=base64:aHR0cHM6Ly93d3cuZ29vZ2xlLmNvbS51YS9pbWFnZXMvc3Jwci9sb2dvMTF3LnBuZw==&size=original
```

## Configuration (local.settings.json or environment variables)
* **SmallSize** - size in pixels for Small predefined value.
* **MediumSize** - size in pixels for Medium predefined value.
* **DetailSize** - size in pixels for Detail predefined value.
* **UserAgent** - user agent string for downloaded images.
* **ClientCache:MaxAge** - time span of client cache max age.

## License
ImageResizer is under the [MIT license](LICENSE.md).
