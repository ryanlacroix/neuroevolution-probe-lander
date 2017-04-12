using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

// Saves the state of an object into XML
public class StateSaver<T> where T: class {
    public string filepath;
    public StateSaver(string path)
    {
        this.filepath = path;
    }

    public bool hasPriorSave()
    {
        return File.Exists(filepath);
    }

    public void save(T pop)
    {
        var ser = new XmlSerializer(typeof(T));
        var stream = new FileStream(filepath, FileMode.Create);
        ser.Serialize(stream, pop);
        stream.Close();
    }

    public T load()
    {
        var ser = new XmlSerializer(typeof(T));
        var stream = new FileStream(filepath, FileMode.Open);
        T returnObj = ser.Deserialize(stream) as T;
        stream.Close();
        return returnObj;
    }
}
