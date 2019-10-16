using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotGameDisplay : Display
{
    public void Back()
    {
        DisplaysManager.instance.ShowDisplay(DisplaysManager.instance.BotsDisplay);
    }
}
