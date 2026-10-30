using System.Linq;
using TMPro;
using UnityEngine;
namespace BWVR {
public sealed partial class QuestSlice {
 GameObject firstSteps;TMP_Text firstStepsHeading,firstStepsBody,firstStepsAction;GameObject firstStepsPrevious,firstStepsNext;int firstStepsPage;
 static readonly string[] FirstStepsTitles={"YOU ARE UNSEEN","INVITE, THEN WATCH","MAKE A PLAY DAY"};
 static readonly string[] FirstStepsBodies={
  "The villagers see the creature as their god. You shape its world from the shadows.\n\nLeft stick: travel. Right stick: turn / height.\nHold right SIDE grip + right stick to zoom.\n\nX opens this scroll. Point and press the right FRONT trigger to choose a button.",
  "Hold the right FRONT trigger to pick something up. Release to place it.\n\nPut a toy in front of the creature, then give it space to choose. A praises its last completed action; B discourages it.\n\nTry playing four notes on the chime while it watches. It may choose to echo your rhythm.",
  "Play Day connects three experiments: a chime echo, shared ball goals and the Together gate.\n\nThe Book explains each one and can mark its location. There is no deadline; your creature still chooses for itself.\n\nOr explore Island Finds and bring a discovery back to share. Save from Sanctuary before leaving."
 };
 void BuildFirstSteps(){
  firstSteps=SanctuaryPage("First steps");
  SanctuaryLabel("First steps title",firstSteps.transform,new Vector2(0,305),new Vector2(530,45),"START HERE",32,ParchmentInk);
  firstStepsHeading=SanctuaryLabel("First steps heading",firstSteps.transform,new Vector2(0,240),new Vector2(530,45),"",27,ParchmentInk);
  firstStepsBody=SanctuaryLabel("First steps body",firstSteps.transform,new Vector2(0,45),new Vector2(510,310),"",23,ParchmentInk);
  firstStepsBody.textWrappingMode=TextWrappingModes.Normal;firstStepsBody.alignment=TextAlignmentOptions.TopLeft;
  firstStepsAction=SanctuaryButton("Try this",firstSteps.transform,new Vector2(0,-155),FirstStepsAct);
  firstStepsPrevious=SanctuaryButton("Previous",firstSteps.transform,new Vector2(-135,-245),()=>ShowFirstSteps(firstStepsPage-1)).transform.parent.gameObject;
  firstStepsNext=SanctuaryButton("Next",firstSteps.transform,new Vector2(135,-245),()=>ShowFirstSteps(firstStepsPage+1)).transform.parent.gameObject;
  SanctuaryButton("Back",firstSteps.transform,new Vector2(0,-325),()=>ShowSanctuaryPage(sanctuaryHome));
 }
 void ShowFirstSteps(int page){
  firstStepsPage=Mathf.Clamp(page,0,2);firstStepsHeading.text=FirstStepsTitles[firstStepsPage];firstStepsBody.text=FirstStepsBodies[firstStepsPage];
  if(!UnityEngine.XR.XRSettings.isDeviceActive)firstStepsBody.text+=firstStepsPage==0?"\nDesktop: Tab menu · F select · T tools.":firstStepsPage==1?"\nDesktop: hold E to carry · 1 / 2 to teach.":"";
  firstStepsAction.text=firstStepsPage==0?"Explore freely":firstStepsPage==1?"Find the chime":"Open Play Day";
  firstStepsPrevious.SetActive(firstStepsPage>0);firstStepsNext.SetActive(firstStepsPage<2);ShowSanctuaryPage(firstSteps);
 }
 void FirstStepsAct(){
  if(firstStepsPage==0){SetSanctuaryOpen(false);return;}
  if(firstStepsPage==2){SelectCreationBook(CreationBookSection.Outings);ShowCreationBook(true);return;}
  if(state.paused||applicationSuspended||held.Any(i=>i!=null)){notice="Resume and release held objects to mark the chime.";return;}
  var target=playerMusic.FirstOrDefault();if(target==null||!target.gameObject.activeInHierarchy){notice="The chime is currently unavailable.";return;}
  ShowActivityBeacon(target.transform);
 }
}
}
