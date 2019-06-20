using System.Configuration;

namespace Pics2Json
{
  public class MergedGalleriesSettingsElement : ConfigurationElement
  {
    [ConfigurationProperty("folder", IsKey = true, IsRequired = true)]
    public string Folder
    {
      get
      {
        return (string)base["folder"];
      }
      set
      {
        base["folder"] = value;
      }
    }

  }
  public class MergedGalleriesSettingsConfig : ConfigurationSection
  {
    [ConfigurationProperty("settings")]
    [ConfigurationCollection(typeof(MergedGalleriesSettingsElementCollection))]
    public MergedGalleriesSettingsElementCollection MergedGalleriesSettingsInstances
    {
      get
      {
        return (MergedGalleriesSettingsElementCollection)this["settings"];
      }
    }
  }

  public class MergedGalleriesSettingsElementCollection : ConfigurationElementCollection
  {
    public MergedGalleriesSettingsElement this[int index]
    {
      get
      {
        return (MergedGalleriesSettingsElement)BaseGet(index);
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
      return new MergedGalleriesSettingsElement();
    }

    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((MergedGalleriesSettingsElement)element).Folder;
    }
  }
}