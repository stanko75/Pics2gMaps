﻿using System.Configuration;

namespace Pics2Json
{
  /*
   * galleryName: Djala
   * rootGalleryFolder: C:\projects\gallery\
   * webPath: http://www.milosev.com/gallery/
   */
  public class GallerySettingsElement : ConfigurationElement
  {
    [ConfigurationProperty("galleryName", IsKey = true, IsRequired = true)]
    public string GalleryName
    {
      get
      {
        return (string)base["galleryName"];
      }
      set
      {
        base["galleryName"] = value;
      }
    }

    [ConfigurationProperty("rootGalleryFolder", IsKey = true, IsRequired = true)]
    public string RootGalleryFolder
    {
      get
      {
        return (string)base["rootGalleryFolder"];
      }
      set
      {
        base["rootGalleryFolder"] = value;
      }
    }

    [ConfigurationProperty("webPath", IsKey = true, IsRequired = true)]
    public string WebPath
    {
      get
      {
        return (string)base["webPath"];
      }
      set
      {
        base["webPath"] = value;
      }
    }

    [ConfigurationProperty("ogTitle", IsKey = true, IsRequired = true)]
    public string OgTitle
    {
      get
      {
        return (string)base["ogTitle"];
      }
      set
      {
        base["ogTitle"] = value;
      }
    }

    [ConfigurationProperty("ogDescription", IsKey = true, IsRequired = true)]
    public string OgDescription
    {
      get
      {
        return (string)base["ogDescription"];
      }
      set
      {
        base["ogDescription"] = value;
      }
    }

    [ConfigurationProperty("ogImage", IsKey = true, IsRequired = true)]
    public string OgImage
    {
      get
      {
        return (string)base["ogImage"];
      }
      set
      {
        base["ogImage"] = value;
      }
    }
  }

  public class GallerySettingsElementCollection : ConfigurationElementCollection
  {
    public GallerySettingsElement this[int index]
    {
      get
      {
        return (GallerySettingsElement)BaseGet(index);
      }
      set
      {
        if (BaseGet(index) != null)
          BaseRemoveAt(index);

        BaseAdd(index, value);
      }
    }

    protected override ConfigurationElement CreateNewElement()
    {
      return new GallerySettingsElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((GallerySettingsElement)element).GalleryName;
    }
  }

  public class GallerySettingsConfig : ConfigurationSection
  {
    [ConfigurationProperty("settings")]
    [ConfigurationCollection(typeof(GallerySettingsElementCollection))]
    public GallerySettingsElementCollection MilosevBlogInstances
    {
      get
      {
        return (GallerySettingsElementCollection)this["settings"];
      }
    }
  }
}