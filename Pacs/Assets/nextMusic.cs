
using UnityEngine;

public class nextMusic : MonoBehaviour {

    public void musiqueSuivante()
    {
        AudioManager.getInstance().nextMusic();
    }

}
