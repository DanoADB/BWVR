using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 Material bookMaterial;Dictionary<string,string> bookTargets;GameObject creationBook;UnityEngine.UI.Text bookHeading,bookBody,bookPageNumber;readonly UnityEngine.UI.Image[] bookTabs=new UnityEngine.UI.Image[4];CreationBookSection bookSection;int bookPage;float bookRefresh;string[] bookPeople;
 public bool CreationBookOpen=>creationBook!=null&&creationBook.activeInHierarchy;
 public bool ShowCreationBook(bool show){
  if(creationBook==null||show&&held.Any(i=>i!=null))return false;
  if(show)godTools.Cancel();
  ShowSanctuaryPage(show?creationBook:sanctuaryHome);if(show){panel.SetActive(true);RefreshCreationBook();}return true;
 }
 public void SelectCreationBook(CreationBookSection section){if(!Enum.IsDefined(typeof(CreationBookSection),section))return;bookSection=section;bookPage=0;RefreshCreationBook();}
 public void TurnCreationBook(int direction){var page=CreationBookText.Read(state,bookSection,bookPage,bookPeople,bookTargets);bookPage=(page.index+(direction<0?-1:1)+page.count)%page.count;RefreshCreationBook();}
 UnityEngine.UI.Text BookLabel(string title,Vector3 position,Vector2 size,int textSize){var label=Label(title,creationBook.transform,position,size,1,textSize);label.color=new Color(.12f,.2f,.18f);label.supportRichText=false;label.raycastTarget=false;label.material=bookMaterial;return label;}
 GameObject BookButton(string title,Vector3 position,float width,Action action){var go=Button(title,position,action,creationBook.transform);go.GetComponent<RectTransform>().sizeDelta=new Vector2(width,58);go.GetComponent<BoxCollider>().size=new Vector3(width,58,15);go.GetComponent<BoxCollider>().isTrigger=true;var text=go.GetComponentInChildren<UnityEngine.UI.Text>();text.rectTransform.sizeDelta=new Vector2(width-8,56);text.fontSize=21;text.supportRichText=false;text.material=bookMaterial;go.GetComponent<UnityEngine.UI.Image>().material=bookMaterial;return go;}
 void BuildCreationBook(){
  bookTargets=hybrid.Targets.Where(t=>t!=null).ToDictionary(t=>t.id,t=>t.name);
  bookPeople=jobs.Select(j=>j.name.Split('(')[0].Trim()).ToArray();creationBook=new GameObject("Book of Creation",typeof(RectTransform),typeof(UnityEngine.UI.Image));creationBook.transform.SetParent(panel.transform,false);creationBook.GetComponent<RectTransform>().sizeDelta=new Vector2(580,720);creationBook.GetComponent<UnityEngine.UI.Image>().color=Color.clear;creationBook.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;creationBook.GetComponent<UnityEngine.UI.Image>().material=bookMaterial;var canvas=creationBook.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.overrideSorting=true;canvas.sortingOrder=100;
  BookLabel("Book title",new Vector3(0,315,0),new Vector2(540,48),30).text="BOOK OF CREATION";
  for(int i=0;i<4;i++){var section=(CreationBookSection)i;bookTabs[i]=BookButton(section.ToString(),new Vector3(-210+i*140,250,0),132,()=>SelectCreationBook(section)).GetComponent<UnityEngine.UI.Image>();}
  bookHeading=BookLabel("Page heading",new Vector3(0,178,0),new Vector2(530,45),28);
  bookBody=BookLabel("Page contents",new Vector3(0,-20,0),new Vector2(520,310),23);bookBody.alignment=TextAnchor.UpperLeft;bookBody.verticalOverflow=VerticalWrapMode.Truncate;
  BookLabel("Read-only note",new Vector3(0,-192,0),new Vector2(530,35),18).text="Recorded history · earlier events may be unknown";
  BookButton("Previous",new Vector3(-170,-245,0),160,()=>TurnCreationBook(-1));bookPageNumber=BookLabel("Page number",new Vector3(0,-245,0),new Vector2(150,54),22);BookButton("Next",new Vector3(170,-245,0),160,()=>TurnCreationBook(1));
  BookButton("Back",new Vector3(0,-320,0),300,()=>ShowCreationBook(false));StyleSanctuarySubview(creationBook);RefreshCreationBook();creationBook.SetActive(false);
 }
 void RefreshCreationBook(){if(creationBook==null)return;var page=CreationBookText.Read(state,bookSection,bookPage,bookPeople,bookTargets);bookPage=page.index;bookHeading.text=page.title;bookBody.text=page.text;bookPageNumber.text=(page.index+1)+" / "+page.count;for(int i=0;i<bookTabs.Length;i++)if(bookTabs[i]!=null)bookTabs[i].color=i==(int)bookSection?new Color(1.2f,1.14f,.85f):Color.white;bookRefresh=.5f;}
 void TickCreationBook(float dt){if(!CreationBookOpen)return;bookRefresh-=dt;if(bookRefresh<=0)RefreshCreationBook();}
 void OnDestroy(){if(sanctuaryStone!=null)Destroy(sanctuaryStone);if(sanctuaryFont!=null)Destroy(sanctuaryFont);if(bookMaterial!=null)Destroy(bookMaterial);}
 bool BookPointer(int side,bool pressed,bool wasPressed,Collider hit,bool tracked){if(!CreationBookOpen)return false;if(side==1&&tracked&&pressed&&!wasPressed&&hit!=null&&hit.transform.IsChildOf(creationBook.transform))hit.GetComponentInParent<SpatialTarget>()?.Activate();return true;}
}
}
