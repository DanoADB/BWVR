using System.IO;
using System.Text;
using System.Xml;
using UnityEditor.Android;
namespace BWVR.Editor {
// Final exported manifest contract, independent of the Editor's selected target
// when this helper is exercised by validation tooling.
public sealed class FoveationManifest:IPostGenerateGradleAndroidProject {
 public int callbackOrder=>10000;
 public void OnPostGenerateGradleAndroidProject(string path)=>EnsureOptionalEyeTracking(Path.Combine(path,"src/main/AndroidManifest.xml"));
 public static void EnsureOptionalEyeTracking(string file){var doc=new XmlDocument();doc.Load(file);var root=doc.DocumentElement;const string android="http://schemas.android.com/apk/res/android";
  XmlElement Ensure(string tag,string name){foreach(XmlNode n in root.GetElementsByTagName(tag))if(n is XmlElement e&&e.GetAttribute("name",android)==name)return e;var item=doc.CreateElement(tag);item.SetAttribute("name",android,name);root.AppendChild(item);return item;}
  Ensure("uses-permission","com.oculus.permission.EYE_TRACKING");Ensure("uses-feature","oculus.software.eye_tracking").SetAttribute("required",android,"false");using(var writer=XmlWriter.Create(file,new XmlWriterSettings{Encoding=new UTF8Encoding(false),Indent=true}))doc.Save(writer);
 }
}
}
