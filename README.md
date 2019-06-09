App.Config should look like this:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>

  <configSections>
    <section name="galeries" type="Pics2Json.GallerySettingsConfig, Pics2Json"></section>
  </configSections>

  <galeries>
    <settings>
      <add galleryName="Djala"
           rootGalleryFolder="C:\projects\gallery\"
           webPath="http://www.milosev.com/gallery/"
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
