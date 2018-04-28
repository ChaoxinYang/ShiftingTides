using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerResourcesManager : MonoBehaviour {

    private int jumpsLeft, health, arrows, dashes;
    private int maxDash = 0, maxJump = 1, maxHealth = 100;
    public float stamina,sourceReserve;
    public Ui ui;
    public PlayerCamera plyCamera;
    // Use this for initialization
    void Start () {
        JumpsLeft = 1;
        Arrows = 1000;
        dashes = 1;
        Health = 100;
    }

    private void Update()
    {      
        transform.rotation = Quaternion.Euler(0,plyCamera.angleH,0);
    } 

    public int JumpsLeft
    {
        get
        {
            return jumpsLeft;
        }
        set
        {
            jumpsLeft = value;
            ui.sliders[0].value = JumpsLeft;
            if
                (jumpsLeft < 0)
            {
                jumpsLeft = 0;
            }
            if (jumpsLeft > maxJump)
            {
                jumpsLeft = maxJump;
            }
        }
    }

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            ui.sliders[1].value = Health;
            if (health < 0)
            {
                health = 0;
            }
            if (health > 100)
            {
                health = 100;
            }
        }
    }
    public int MaxJump
    {
        get { return maxJump; }
        set { maxJump = value; if (maxJump > 3) { maxJump = 3; } else if (maxJump < 1) { maxJump = 1; } }
    }
    public int MaxDash
    {
        get { return maxDash; }
        set { maxDash = value; if (maxDash > 3) { maxDash = 3; } else if (maxDash < 1) { maxDash = 1; } }
    }
    public float Stamina
    {
        get
        {
            return stamina;
        }
        set
        {
            stamina = value;
            if (stamina < 0)
            {
                stamina = 0;
            }
            if (stamina > 100)
            {
                stamina = 100;
            }
        }
    }

    public int Arrows
    {
        get
        {
            return arrows;
        }
        set
        {
            arrows = value;
            ui.texts[0].text = Arrows.ToString();
            if (arrows < 0)
            {
                arrows = 0;
            }
            else if (arrows > 20000)
            {
                arrows = 20000;
            }
        }
    }

    public int Dashes
    {
        get
        {
            return dashes;
        }
        set
        {
            dashes = value;
            if (dashes < 0)
            {
                dashes = 0;
            }
            else if (dashes > maxDash)
            {
                dashes = maxDash;
            }
        }
    }

    public float SourceReserve { get { return sourceReserve; } set { sourceReserve = value; } }
}
