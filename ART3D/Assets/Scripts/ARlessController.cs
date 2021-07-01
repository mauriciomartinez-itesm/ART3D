using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ARlessController : MonoBehaviour
{
    [SerializeField]
    private ScrollRectScript scrollrectScript;
    public UX_Helper _ux_Helper;


    public void ARSwitch(string sceneName)
    {
        _ux_Helper.ActiveAR(sceneName);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void filterBy()
    {
        var favAct = Resources.Load<Sprite>("Sprites/favbtnOs");
        var Unfav = Resources.Load<Sprite>("Sprites/heartfav");

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
