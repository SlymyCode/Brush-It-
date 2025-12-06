using UnityEngine;
using UnityEngine.UI;

public class ResolutionChange : MonoBehaviour
{
   public Toggle fullScreenToggle;
   private bool fullscreen = true;
   
   void Start()
   {
      fullScreenToggle.isOn = true;
   }
   
   public void EnableToggle()
   {
      fullScreenToggle.isOn = true;
   }

   public void Fullscreen()
   {
      fullscreen = !fullscreen;
      Screen.fullScreen = fullscreen;
   }
   
   public void Resolution1()
   {
      EnableToggle();
      Screen.SetResolution(1920, 1080, fullscreen);
   }

   public void Resolution2()
   {
      EnableToggle();
      Screen.SetResolution(2560, 1440, fullscreen);
   }

   public void Resolution3()
   {
      EnableToggle();
      Screen.SetResolution(3840, 2160, fullscreen);
   }
}
