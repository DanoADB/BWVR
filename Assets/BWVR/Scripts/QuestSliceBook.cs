using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 Material bookMaterial;Dictionary<string,string> bookTargets;GameObject creationBook;UnityEngine.UI.Text bookHeading,bookBody,bookPageNumber;readonly UnityEngine.UI.Image[] bookTabs=new UnityEngine.UI.Image[6];CreationBookSection bookSection=CreationBookSection.Outings;int bookPage;float bookRefresh;string[] bookPeople;
 GameObject outingGo,outingEnd,bookPrevious,bookNext,bookSections;bool bookIndexOpen;UnityEngine.UI.Text outingGoCaption,bookReadNote;
 public bool CreationBookOpen=>creationBook!=null&&creationBook.activeInHierarchy;
 public bool ShowCreationBook(bool show){
  if(creationBook==null||show&&held.Any(i=>i!=null))return false;
  if(show)godTools.Cancel();
  ShowSanctuaryPage(show?creationBook:sanctuaryHome);if(show){panel.SetActive(true);RefreshCreationBook();}return true;
 }
 public void SelectCreationBook(CreationBookSection section){if(!Enum.IsDefined(typeof(CreationBookSection),section))return;bookSection=section;bookIndexOpen=false;bookPage=0;RefreshCreationBook();}
 public void TurnCreationBook(int direction){var page=CreationBookText.Read(state,bookSection,bookPage,bookPeople,bookTargets);bookPage=(page.index+(direction<0?-1:1)+page.count)%page.count;RefreshCreationBook();}
 UnityEngine.UI.Text BookLabel(string title,Vector3 position,Vector2 size,int textSize){var label=Label(title,creationBook.transform,position,size,1,textSize);label.color=new Color(.12f,.2f,.18f);label.supportRichText=false;label.raycastTarget=false;label.material=bookMaterial;return label;}
 GameObject BookButton(string title,Vector3 position,Action action){var go=Button(title,position,action,creationBook.transform);go.GetComponent<BoxCollider>().isTrigger=true;var text=go.GetComponentInChildren<UnityEngine.UI.Text>();text.supportRichText=false;text.material=bookMaterial;go.GetComponent<UnityEngine.UI.Image>().material=bookMaterial;AddBookIcon(go,BookIcon(title));return go;}
 GameObject OutingButton(string title,float x,Action action)=>BookButton(title,new Vector3(x,-162,0),action);
 void BuildCreationBook(){
  bookTargets=hybrid.Targets.Where(t=>t!=null).ToDictionary(t=>t.id,t=>t.name);
  bookPeople=jobs.Select(j=>j.name.Split('(')[0].Trim()).ToArray();creationBook=new GameObject("Book of Creation",typeof(RectTransform),typeof(UnityEngine.UI.Image));creationBook.transform.SetParent(panel.transform,false);creationBook.GetComponent<RectTransform>().sizeDelta=new Vector2(580,720);creationBook.GetComponent<UnityEngine.UI.Image>().color=Color.clear;creationBook.GetComponent<UnityEngine.UI.Image>().raycastTarget=false;creationBook.GetComponent<UnityEngine.UI.Image>().material=bookMaterial;var canvas=creationBook.AddComponent<Canvas>();canvas.renderMode=RenderMode.WorldSpace;canvas.overrideSorting=true;canvas.sortingOrder=100;
  BookLabel("Book title",new Vector3(0,315,0),new Vector2(540,48),30).text="BOOK OF CREATION";
  for(int i=0;i<bookTabs.Length;i++){var section=(CreationBookSection)i;var tab=BookButton(section.ToString(),new Vector3(i%2==0?-135:135,230-(i/2)*90,0),()=>SelectCreationBook(section));bookTabs[i]=tab.GetComponent<UnityEngine.UI.Image>();}
  bookHeading=BookLabel("Page heading",new Vector3(0,245,0),new Vector2(530,45),28);
  bookBody=BookLabel("Page contents",new Vector3(0,45,0),new Vector2(520,310),23);bookBody.alignment=TextAnchor.UpperLeft;bookBody.verticalOverflow=VerticalWrapMode.Truncate;
  bookReadNote=BookLabel("Read-only note",new Vector3(0,-162,0),new Vector2(530,35),18);bookReadNote.text="Recorded history · earlier events may be unknown";
  outingGo=OutingButton("Begin Play Day",-135,()=>{if(bookSection==CreationBookSection.Finds)MarkFindPage(bookPage);else if(state.playOuting?.phase==PlayOutingPhase.Active)MarkPlayActivity(bookPage);else BeginPlayOuting();RefreshCreationBook();});outingGoCaption=outingGo.GetComponentInChildren<UnityEngine.UI.Text>();outingGoIcon=outingGo.GetComponentInChildren<UnityEngine.UI.RawImage>();
  outingEnd=OutingButton("End outing",135,()=>{EndPlayOuting();RefreshCreationBook();});
  bookPrevious=BookButton("Previous",new Vector3(-135,-258,0),()=>TurnCreationBook(-1));bookPageNumber=BookLabel("Page number",new Vector3(0,-211,0),new Vector2(150,24),20);bookNext=BookButton("Next",new Vector3(135,-258,0),()=>TurnCreationBook(1));
  bookSections=BookButton("Sections",new Vector3(-135,-325,0),()=>{bookIndexOpen=!bookIndexOpen;RefreshCreationBook();});BookButton("Back",new Vector3(135,-325,0),()=>ShowCreationBook(false));bookSectionsIcon=bookSections.GetComponentInChildren<UnityEngine.UI.RawImage>();StyleSanctuarySubview(creationBook);LayoutBookIcons();RefreshCreationBook();creationBook.SetActive(false);
 }
 void RefreshCreationBook(){if(creationBook==null)return;var page=CreationBookText.Read(state,bookSection,bookPage,bookPeople,bookTargets);bookPage=page.index;bookReadNote.text=bookSection==CreationBookSection.Places&&bookPage==page.count-1?"Physical invitation · creature chooses independently":"Recorded history · earlier events may be unknown";bookHeading.text=page.title;bookBody.text=page.text;bookPageNumber.text=(page.index+1)+" / "+page.count;for(int i=0;i<bookTabs.Length;i++)if(bookTabs[i]!=null){bookTabs[i].gameObject.SetActive(bookIndexOpen);bookTabs[i].color=i==(int)bookSection?new Color(1.2f,1.14f,.85f):Color.white;}bookHeading.gameObject.SetActive(!bookIndexOpen);bookBody.gameObject.SetActive(!bookIndexOpen);bookPageNumber.gameObject.SetActive(!bookIndexOpen);bookPrevious.SetActive(!bookIndexOpen);bookNext.SetActive(!bookIndexOpen);bookSections.GetComponentInChildren<UnityEngine.UI.Text>().text=bookIndexOpen?"Return to page":"Sections";bool outing=!bookIndexOpen&&bookSection==CreationBookSection.Outings;bool finds=!bookIndexOpen&&bookSection==CreationBookSection.Finds;bookReadNote.gameObject.SetActive(!bookIndexOpen&&!outing&&!finds);outingGo.SetActive(outing||finds);outingEnd.SetActive(outing&&state.playOuting?.phase==PlayOutingPhase.Active);outingGoCaption.text=finds?(state.paused?"Resume to mark":"Mark clearing"):state.paused?"Resume to begin / mark":state.playOuting?.phase==PlayOutingPhase.Active?"Mark activity":state.playOuting?.phase==PlayOutingPhase.Complete?"Play again":"Begin Play Day";RefreshBookIcons();bookRefresh=.5f;}
 void TickCreationBook(float dt){if(!CreationBookOpen)return;bookRefresh-=dt;if(bookRefresh<=0)RefreshCreationBook();}
 void OnDestroy(){DisposeTreasureTrail();DisposePlayOuting();Chapter?.Dispose();if(sanctuaryStone!=null)Destroy(sanctuaryStone);if(sanctuaryFont!=null)Destroy(sanctuaryFont);if(bookMaterial!=null)Destroy(bookMaterial);}
 bool BookPointer(int side,bool pressed,bool wasPressed,Collider hit,bool tracked){if(!CreationBookOpen)return false;if(side==1&&tracked&&pressed&&!wasPressed&&hit!=null&&hit.transform.IsChildOf(creationBook.transform))hit.GetComponentInParent<SpatialTarget>()?.Activate();return true;}
}
}
