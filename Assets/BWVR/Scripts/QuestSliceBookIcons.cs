using System;
using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 Texture2D bookIconAtlas;UnityEngine.UI.RawImage outingGoIcon,bookSectionsIcon;
 static readonly string[] BookIconNames={"paw-print","users","package","map","footprints","chevron-left","chevron-right","layout-grid","log-out","play","map-pin","square","rotate-ccw","book-open"};
 static string BookIcon(string button){switch(button){case "Creature":return "paw-print";case "People":return "users";case "Provisions":return "package";case "Places":return "map";case "Outings":return "footprints";case "Previous":return "chevron-left";case "Next":return "chevron-right";case "Sections":return "layout-grid";case "Back":return "log-out";case "End outing":return "square";default:return "play";}}
 UnityEngine.UI.RawImage AddBookIcon(GameObject button,string name){
  if(bookIconAtlas==null)bookIconAtlas=Resources.Load<Texture2D>("BookIcons/atlas");if(bookIconAtlas==null)throw new InvalidOperationException("Missing Book icon atlas");
  var go=new GameObject("Book button icon",typeof(RectTransform),typeof(UnityEngine.UI.RawImage));go.transform.SetParent(button.transform,false);go.transform.localPosition=new Vector3(-88,0,-48);var image=go.GetComponent<UnityEngine.UI.RawImage>();image.texture=bookIconAtlas;image.color=StoneInk;image.material=bookMaterial;image.raycastTarget=false;image.rectTransform.sizeDelta=new Vector2(32,32);SetBookIcon(image,name);return image;
 }
 static void SetBookIcon(UnityEngine.UI.RawImage image,string name){int index=Array.IndexOf(BookIconNames,name);if(index<0)throw new ArgumentOutOfRangeException(nameof(name));image.uvRect=new Rect((index%4)*.25f,1-(index/4+1)*.25f,.25f,.25f);}
 void LayoutBookIcons(){foreach(var target in creationBook.GetComponentsInChildren<SpatialTarget>(true)){var text=target.GetComponentInChildren<UnityEngine.UI.Text>();text.rectTransform.localPosition=new Vector3(20,0,-48);text.rectTransform.sizeDelta=new Vector2(166,52);}}
 void RefreshBookIcons(){SetBookIcon(outingGoIcon,state.playOuting?.phase==PlayOutingPhase.Active?"map-pin":state.playOuting?.phase==PlayOutingPhase.Complete?"rotate-ccw":"play");SetBookIcon(bookSectionsIcon,bookIndexOpen?"book-open":"layout-grid");}
}
}
