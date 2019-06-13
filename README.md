Extracts GPS position from images, save it to JSON and display them on google maps.

App.Config should look like this:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>

  <configSections>
    <section name="galeries" type="Pics2Json.GallerySettingsConfig, Pics2Json"></section>
  </configSections>

  <galeries>
    <settings>
      <add galleryName="djala"
           rootGalleryFolder="C:\projects\gallery\allWithPics"
           webPath="http://www.milosev.com/gallery/allWithPics/"
           ogTitle="Đala - place where my father was born"
           ogDescription="Few pictures of village from where my father came."
           ogImage="thumbs/IMAG0091.jpg"
           zoom="14"
           joomlaThumbsPath="/gallery/allWithPics/djala/www/djalaThumbs.json"
           joomlaImgSrcPath="/gallery/allWithPics/djala/www/"
      />
      <add galleryName="altenahr"
           rootGalleryFolder="C:\projects\gallery\allWithPics"
           webPath="http://www.milosev.com/gallery/allWithPics/"
           ogTitle="Altenahr"
           ogDescription="Wanderspaß und Weingenuss in Altenahr."
           ogImage="thumbs/20170924_155342.jpg"
           zoom="14"
           joomlaThumbsPath="/gallery/allWithPics/altenahr/www/altenahrThumbs.json"
           joomlaImgSrcPath="/gallery/allWithPics/altenahr/www/"
      />
      <add galleryName="easter2019"
           rootGalleryFolder="C:\projects\gallery\allWithPics\travelBuddies"
           webPath="http://www.milosev.com/gallery/allWithPics/travelBuddies/"
           ogTitle="Easter 2019"
           ogDescription="Easter 2019."
           ogImage="thumbs/20190414_161631.jpg"
           zoom="7"
           joomlaThumbsPath="/gallery/allWithPics/travelBuddies/easter2019/www/easter2019Thumbs.json"
           joomlaImgSrcPath="/gallery/allWithPics/travelBuddies/easter2019/www/"
      />
      <add galleryName="vacation2018"
           rootGalleryFolder="C:\projects\gallery\allWithPics\travelBuddies"
           webPath="http://www.milosev.com/gallery/allWithPics/travelBuddies/"
           ogTitle="Vacation 2018"
           ogDescription="Vacation 2018."
           ogImage="thumbs/20180707_103455.jpg"
           zoom="5"
           joomlaThumbsPath="/gallery/allWithPics/travelBuddies/vacation2018/www/vacation2018Thumbs.json"
           joomlaImgSrcPath="/gallery/allWithPics/travelBuddies/vacation2018/www/"
      />      
      <add galleryName="travelBuddies"
           rootGalleryFolder="C:\projects\gallery\allWithPics"
           webPath="http://www.milosev.com/gallery/allWithPics/"
           ogTitle="Ella and Stanko"
           ogDescription="Ella and Stanko - travel buddies"
           ogImage="easter2019/thumbs/20190414_161631.jpg"
           zoom="5"
           joomlaThumbsPath="/gallery/allWithPics/travelBuddies/www/travelBuddiesThumbs.json"
           joomlaImgSrcPath="/gallery/allWithPics/travelBuddies/www/"
      />
      <add galleryName="allWithPics"
           rootGalleryFolder="C:\projects\gallery\"
           webPath="http://www.milosev.com/gallery/"
           ogTitle="List of all places"
           ogDescription="List of all places which are published."
           ogImage="travelBuddies/easter2019/thumbs/20190414_161631.jpg"
           zoom="5"
           joomlaThumbsPath="/gallery/allWithPics/www/allWithPicsThumbs.json"
           joomlaImgSrcPath="/gallery/allWithPics/www/"
      />
    </settings>
  </galeries>

  <appSettings>
    <add key="gapikey" value="myApiKey" />
  </appSettings>

  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.6.1" />
  </startup>
</configuration>
```
Folder structure should look like:

![alt text](https://github.com/stanko75/Pics2gMaps/blob/master/folderStructure.png "Folder structure")


Notice that all galleries are under allWithPics, and easter2019 and vacation2018 are under travelBuddies. That means that galleries easter2019 and vacation2018 will be merged in travelBuddies gallery, and all galleris will be merged under allWithPics.

For time being my personal web site is hardcoded almost everywhere in solution.

Examples: 

[Joomla!](http://milosev.com/2015-01-23-20-08-55/gallery.html)

[Travel buddies](http://milosev.com/gallery/allWithPics/travelBuddies/www/index.html)

[Vacation 2018](http://milosev.com/gallery/allWithPics/travelBuddies/vacation2018/www/index.html)

[Easter 2019](http://milosev.com/2015-01-23-20-08-55/gallery/523-easter-2019.html)
