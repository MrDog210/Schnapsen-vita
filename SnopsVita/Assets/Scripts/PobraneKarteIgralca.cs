using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PobraneKarteIgralca : MonoBehaviour
{
    public List<GameObject> Karte;
    public string lastnik;
    public int tocke;
    public int tockZmage = 0;
    private void Awake()
    {
        tocke = 0;
        tockZmage = 0;
    }

    public void SprejemiNoveKarte(List<GameObject> noveKarte)
    {
        CardScript kartaScript;
        Vector3 novaPoz;
        for (int i = 0; i < noveKarte.Count; i++) { 
            kartaScript = pridobiScriptKarte(noveKarte[i]);
            kartaScript.jeObrnjeno = true;
            kartaScript.obrniKarto();
            Karte.Add(noveKarte[0]);
            if (Karte.Count == 1)
            {
                kartaScript.premakniKarto(gameObject.transform.position, Quaternion.Euler(0, 0, Random.Range(-15, 15)));
            } else
            {
                novaPoz = gameObject.transform.position;
                novaPoz.z = Karte[Karte.Count - 2].transform.position.z;
                novaPoz.z -= 0.1f;
                kartaScript.premakniKarto(novaPoz, Quaternion.Euler(0, 0, Random.Range(-15, 15)));
            }
            tocke += kartaScript.vrednost;
        }
    }

    public int PridobiVrednostPK()
    {
        int vrednost=0;
        CardScript sk;
        for(int i = 0; i < Karte.Count; i++)
        {
            sk = pridobiScriptKarte(Karte[i]);
            vrednost += sk.vrednost;
        }
        return vrednost;
    }

    public CardScript pridobiScriptKarte(GameObject kartaGO)
    {
        return (CardScript)kartaGO.GetComponent(typeof(CardScript));
    }
}
